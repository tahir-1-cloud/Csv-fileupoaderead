using Microsoft.AspNetCore.Mvc;
using VacationPark.Interface;
using VacationPark.Models;

namespace VacationPark.Controllers
{
    public class ReservationsController : Controller
    {
        private readonly IParkRepository _parkRepository;
        private readonly IHouseRepository _houseRepository;
        private readonly IReservationRepository _reservationRepository;

        public ReservationsController(IParkRepository parkRepository, IHouseRepository houseRepository, IReservationRepository reservationRepository)
        {
            _parkRepository = parkRepository;
            _houseRepository = houseRepository;
            _reservationRepository = reservationRepository;
        }

        // GET: Create Reservation
        public ActionResult Create()
        {
            ViewBag.Parks = _parkRepository.GetAllParks();
            return View();
        }

        // POST: Create Reservation
        [HttpPost]
        public ActionResult Create(Reservation reservation, int parkId, int capacity)
        {
            if (ModelState.IsValid)
            {
                var availableHouses = _houseRepository.GetAvailableHouses(parkId, capacity);
                if (!availableHouses.Any())
                {
                    ModelState.AddModelError("", "No available houses match the criteria.");
                    ViewBag.Parks = _parkRepository.GetAllParks();
                    return View(reservation);
                }

                reservation.HouseID = availableHouses.First().HouseID;
                _reservationRepository.AddReservation(reservation); // Ensure this method works correctly
                return RedirectToAction("Index");
            }

            ViewBag.Parks = _parkRepository.GetAllParks();
            return View(reservation);
        }


        // GET: Search Reservations
        public ActionResult SearchResult()
        {
            ViewBag.Parks = _parkRepository.GetAllParks();
            return View();
        }

        // POST: Search Reservations
        [HttpPost]
        public ActionResult Search(string customerName, DateTime? startDate, DateTime? endDate, int? parkId)
        {
            var reservations = _reservationRepository.GetReservationsByCriteria(parkId, customerName, startDate, endDate);
            return View("SearchResult", reservations);
        }



        [HttpGet]
        public IActionResult Searchbuton()
        {
            ViewBag.Parks = _parkRepository.GetAllParks();
            return View();
        }
    }
}
