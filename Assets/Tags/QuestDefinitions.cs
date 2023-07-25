#nullable enable
using System.Collections.Generic;
using XVNML.Core.Tags;
using XVNML.Utilities.Tags;

namespace XVNML2U.Tags
{
    [AssociateWithTag("questDefinitions", new[] { typeof(Proxy), typeof(Source) }, TagOccurance.PragmaOnce, true)]
    public sealed class QuestDefinitions : TagBase
    {
        protected override string[] AllowedFlags => new[]
        {
            "useCategories"
        };

        private SortedDictionary<string, (QuestCategory? category, Quest[] quests)>? _questCategoryMap;

        public SortedDictionary<string, (QuestCategory? category, Quest[] quests)>? QuestCategoryMap => _questCategoryMap;

        public override void OnResolve(string? fileOrigin)
        {
            base.OnResolve(fileOrigin);
            _questCategoryMap = new SortedDictionary<string, (QuestCategory? category, Quest[] quests)>();

            var onlyUseCategories = HasFlag(AllowedFlags[0]);
            if (onlyUseCategories == false)
            {
                _questCategoryMap.Add("default", (null, Collect<Quest>()));
                return;
            }

            var questCategories = Collect<QuestCategory>();
            foreach (var category in questCategories)
            {
                var categoryName = category.TagName ?? category.TagID.ToString();
                _questCategoryMap.Add(categoryName, (category, category.Quests));
            }
        }
    }
}