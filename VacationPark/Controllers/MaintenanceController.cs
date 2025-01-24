using Microsoft.AspNetCore.Mvc;
using VacationPark.Interface;

namespace VacationPark.Controllers
{
    public class MaintenanceController : Controller
    {
        private readonly IHouseRepository _houseRepository;
        private readonly IReservationRepository _reservationRepository;

        public MaintenanceController(IHouseRepository houseRepository, IReservationRepository reservationRepository)
        {
            _houseRepository = houseRepository;
            _reservationRepository = reservationRepository;
        }

        // GET: Mark House Under Maintenance
        public ActionResult MarkUnderMaintenance(int houseId)
        {
            var reservations = _reservationRepository.GetReservationsByCriteria(null, null, null, null)
                .Where(r => r.HouseID == houseId);

            ViewBag.AffectedReservations = reservations;

            var house = _houseRepository.GetHousesByPark(houseId).FirstOrDefault(h => h.HouseID == houseId);
            if (house != null)
            {
                house.IsActive = false;
                _houseRepository.MarkHouseUnderMaintenance(houseId);
            }

            return View();
        }
    }
}

