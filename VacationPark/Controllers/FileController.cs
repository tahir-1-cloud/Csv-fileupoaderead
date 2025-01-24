using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VacationPark.BusinesServices;
using VacationPark.Interface;
using VacationPark.Models;

namespace VacationPark.Controllers
{
    public class FileUploadController : Controller
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IFacilityRepository _facilityRepository;
        private readonly IHouseRepository _houseRepository;
        private readonly IParkRepository _parkRepository;
        private readonly IReservationRepository _reservationRepository;


        public FileUploadController(
            ICustomerRepository customerRepository,
            IFacilityRepository facilityRepository,
            IHouseRepository houseRepository,
            IParkRepository parkRepository,
            IReservationRepository reservationRepository
            )
        {
            _customerRepository = customerRepository;
            _facilityRepository = facilityRepository;
            _houseRepository = houseRepository;
            _parkRepository = parkRepository;
            _reservationRepository = reservationRepository;
        }


        //Function for uoloading file
        public IActionResult Upload()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                ModelState.AddModelError("", "Please select a valid file.");
                return View();
            }

            int recordsProcessed = 0;

            using (var stream = new StreamReader(file.OpenReadStream()))
            {
                string line;
                while ((line = stream.ReadLine()) != null)
                {
                    // Split by both ',' (comma) and '|' (pipe)
                    string[] data = line.Contains('|') ? line.Split('|') : line.Split(',');

                    // Dynamically determine which table to insert into based on data format
                    if (data.Length == 3) // Customer format
                    {
                        var customer = new Customer
                        {
                            CustomerID = int.Parse(data[0]),
                            Name = data[1],
                            Address = data[2]
                        };
                        _customerRepository.AddCustomer(customer);
                        recordsProcessed++;
                    }
                    else if (data.Length == 2) // Facility format
                    {
                        var facility = new Facility
                        {
                            FacilityID = int.Parse(data[0]),
                            Description = data[1]
                        };
                        _facilityRepository.AddFacility(facility);
                        recordsProcessed++;
                    }
                    else if (data.Length == 5) // House format
                    {
                        var house = new House
                        {
                            HouseID = int.Parse(data[0]),
                            Street = data[1],
                            Number = int.Parse(data[2]),
                            IsActive = bool.Parse(data[3]),
                            Capacity = int.Parse(data[4])
                        };
                        _houseRepository.AddHouse(house);
                        recordsProcessed++;
                    }
                   
                 
                    else if (data.Length == 3) // Park format
                    {
                        var park = new Park
                        {
                            ParkID = int.Parse(data[0]),
                            Name = data[1],
                            Location = data[2]
                        };
                        _parkRepository.AddPark(park);
                        recordsProcessed++;
                    }
                    else if (data.Length == 4) // Reservation format
                    {
                        if (DateTime.TryParseExact(data[1], "d/M/yyyy H:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out var startDate) &&
                            DateTime.TryParseExact(data[2], "d/M/yyyy H:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out var endDate))
                        {
                            var reservation = new Reservation
                            {
                                ReservationID = int.Parse(data[0]),
                                StartDate = startDate,
                                EndDate = endDate,
                                CustomerID = int.Parse(data[3]),
                            };
                            _reservationRepository.AddReservation(reservation);
                            recordsProcessed++;
                        }
                        else
                        {
                            // Log or handle the invalid date format
                            Console.WriteLine($"Invalid date format in record: {string.Join(",", data)}");
                            ModelState.AddModelError("", $"Invalid date format in record: {string.Join(",", data)}");
                        }
                    }
                    else
                    {
                        // Handle unexpected data formats
                        Console.WriteLine($"Invalid data format in line: {line}");
                        ModelState.AddModelError("", $"Invalid data format in line: {line}");
                    }
                }
            }

            TempData["Message"] = $"{recordsProcessed} records were successfully processed.";
            return RedirectToAction("ShowData");
        }


        // function for show all table data 
        [HttpGet]
        public IActionResult ShowData()
        {
            var customers = _customerRepository.GetAllCustomers();
            var facilities = _facilityRepository.GetAllFacilities();
            var houses = _houseRepository.GetAllHouses();
            var parks = _parkRepository.GetAllParks();
            var reservations = _reservationRepository.GetAllReservations();

            var model = new UploadResultModel
            {
                Customers = customers,
                Facilities = facilities,
                Houses = houses,
                Parks = parks,
                Reservations = reservations
            };

            return View(model);
        }
    }
}
