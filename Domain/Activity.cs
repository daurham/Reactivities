// IMPORTANT NOTE: This is a "Domain Entity" aka a "Model"
// Must be public props to work.
namespace Domain
{
    public class Activity
    {
        // [Key] // Add this attribute if you don't want to use the name "Id" as the database key ID.
        public Guid Id { get; set; } // Since its named Id, the entity framework will automatically know how to use this in db
        public string Title { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public string City { get; set; }
        public string Venue { get; set; }
    }
}