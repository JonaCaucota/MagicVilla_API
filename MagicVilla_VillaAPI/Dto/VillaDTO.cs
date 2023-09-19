using System.ComponentModel.DataAnnotations;

namespace MagicVilla_VillaAPI.Dto
{
    public class VillaDTO
    {
        [Required]
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedDate { get; set; }
        public int Occupancy { get; set; }
        public int Sqft { get; set; }
        public string ImageUrl { get; set; }
        public string Amenity { get; set; }
        public string Details { get; set; }
        [Required]
        public double Rate { get; set; }

    }
}
