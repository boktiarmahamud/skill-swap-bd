namespace SkillSwapBD.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<Skill> Skills { get; set; } = new List<Skill>();
    }
}
