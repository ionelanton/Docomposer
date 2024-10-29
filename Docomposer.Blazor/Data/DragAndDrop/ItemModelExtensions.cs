using Docomposer.Core.Domain;

namespace Docomposer.Blazor.Data.DragAndDrop
{
    public static class ItemModelExtensions
    {
        public static ItemAvailableModel ToItemAvailableModel(this SectionDocument sectionDocument)
        {
            return new ItemAvailableModel
            {
                Id = sectionDocument.Id,
                Name = sectionDocument.Name
            };
        }

        public static ItemIncludedModel ToItemInclucedModel(this SectionDocument sectionDocument)
        {
            return new ItemIncludedModel
            {
                Id = sectionDocument.Id,
                Name = sectionDocument.Name,
                PredecessorId = sectionDocument.PredecessorId
            };
        }
    }
}