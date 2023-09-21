using System.ComponentModel.DataAnnotations;

namespace MagicVilla_VillaAPI.Dto
{
    public class VillaUpdateDTO
    {
        [Required]
        public int Id { get; set; }
        [Required]

        public string Name { get; set; }
        public DateTime CreatedDate { get; set; }
        [Required]
        public int Occupancy { get; set; }
        [Required]

        public int Sqft { get; set; }
        public string ImageUrl { get; set; }
        [Required]

        public string Amenity { get; set; }
        [Required]

        public string Details { get; set; }
        [Required]
        public double Rate { get; set; }

    }
}
