using System;

namespace StoryVerse.Core.Models
{
    public interface IEntity
    {
        Guid Id { get; }
        void Validate();
        void Refresh();
        void Update();
        void CreateAndFlush();
        void UpdateAndFlush();
        void DeleteAndFlush();
    }
}