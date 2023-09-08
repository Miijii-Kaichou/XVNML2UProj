#nullable enable
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using XVNML2U.Tags;
using XVNML2U.Data;

namespace XVNML2U.Mono
{
    public sealed class XVNMLQuestSystem : Singleton<XVNMLQuestSystem>
    {
        [SerializeField]
        private XVNMLModule? module;

        [Header("Quest Unity Events"), Space(2)]
        [Space(2), SerializeField] private UnityEvent<QuestLog> _onQuestInitialize;
        [Space(2), SerializeField] private UnityEvent<QuestLog> _onQuestActive;
        [Space(2), SerializeField] private UnityEvent<QuestLog> _onQuestInactive;
        [Space(2), SerializeField] private UnityEvent<QuestLog> _onQuestComplete;
        [Space(2), SerializeField] private UnityEvent<QuestLog> _onNextTask;
        [Space(2), SerializeField] private UnityEvent<QuestLog> _onTaskComplete;

        public static SortedDictionary<(string category, string id), QuestLog?>? QuestControl { get; private set; }
        public static QuestLog?[] ActiveQuests =>
            QuestControl
            .Where(qc => qc.Value!.Active)
            .Select(qc => qc.Value)
            .ToArray();


        public static string DefaultCategory { get; private set; } = "default";
        private static bool DefaultSet = false;

        public static UnityEvent<QuestLog> OnQuestInitilize => Instance._onQuestInitialize;
        public static UnityEvent<QuestLog> OnQuestActive => Instance._onQuestActive;
        public static UnityEvent<QuestLog> OnQuestInactive => Instance._onQuestInactive;
        public static UnityEvent<QuestLog> OnQuestComplete => Instance._onQuestComplete;
        public static UnityEvent<QuestLog> OnNextTask => Instance._onNextTask;
        public static UnityEvent<QuestLog> OnTaskComplete => Instance._onTaskComplete;

        public static bool IsInitialized { get; private set; }

        public static void Init(XVNMLModule module)
        {
            if (IsNull) return;
            if (IsInitialized) return;

            Instance.module ??= module;

            var questDefinitions = module.Get<QuestDefinitions>();
            if (questDefinitions == null) return;

            var questCategories = questDefinitions.QuestCategoryMap;
            if (questCategories == null) return;

            QuestControl = new SortedDictionary<(string category, string id), QuestLog>();

            foreach(var category in questCategories)
            {
                if (category.Value.category.IsDefault && DefaultSet == false)
                {
                    DefaultSet = !DefaultSet;
                    DefaultCategory = category.Key;
                }

                CreateNewQuestLogCategory(category);
            }

            IsInitialized = true;
        }

        public static void InitializeQuest(string questID, string? questCategory)
        {
            questCategory ??= DefaultCategory;
            QuestControl?[(questCategory, questID)]?.Initialize();
        }

        public static void SetQuestActive(bool active, string questID, string? questCategory)
        {
            questCategory ??= DefaultCategory;
            QuestControl?[(questCategory, questID)]?.SetActive(active);
        }

        public static void CompleteCurrentTask(string questID, string? questCategory)
        {
            questCategory ??= DefaultCategory;
            QuestControl?[(questCategory, questID)]?.CompleteCurrentTask();
        }

        public static void SaveQuestSystemState()
        {
            //Save Quest System State as JSON
        }

        public static void LoadQuestSystemState()
        {
            //Load Quest System State
        }

        private static void CreateNewQuestLogCategory(KeyValuePair<string, (QuestCategory, Quest[])> category)
        {
            string categoryID = category.Key;

            List<QuestLog> quests = new List<QuestLog>();

            foreach(var quest in category.Value.Item2)
            {
                QuestLog newLog = new()
                {
                    questName = quest.Title ?? "Untitled",
                    questDescription = quest.Description,
                    questCategoryName = categoryID
                };

                newLog.GenerateQuestTasks(quest.Objectives);

                newLog.onQuestInitialize += () => Instance._onQuestInitialize.Invoke(newLog);
                newLog.onQuestActive += () => Instance._onQuestActive.Invoke(newLog);
                newLog.onQuestInActive += () => Instance._onQuestInitialize.Invoke(newLog);
                newLog.onQuestComplete += () => Instance._onQuestComplete.Invoke(newLog);

                newLog.onNextTask += () => Instance._onNextTask.Invoke(newLog);
                newLog.onTaskComplete += () => Instance._onTaskComplete.Invoke(newLog);

                QuestControl.Add((categoryID, quest.TagName ?? quest.TagID.ToString()), newLog);
            }
        }
    }
}
