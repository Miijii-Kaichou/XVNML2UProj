using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using XVNML2U.Mono;
using XVNML2U.Tags;

namespace XVNML2U
{
    public sealed class XVNMLQuestSystem : Singleton<XVNMLQuestSystem>
    {
        [SerializeField]
        private XVNMLModule module;

        [Header("Quest Unity Events")]
        [SerializeField] private UnityEvent _onQuestInitialize;
        [SerializeField] private UnityEvent _onQuestActive;
        [SerializeField] private UnityEvent _onQuestInactive;
        [SerializeField] private UnityEvent _onQuestComplete;
        [SerializeField] private UnityEvent _onNextTask;
        [SerializeField] private UnityEvent _onTaskComplete;

        private static SortedDictionary<(string, string), QuestLog> QuestControl;

        public static QuestLog[] ActiveQuests =>
            QuestControl
            .Where(qc => qc.Value.Active)
            .Select(qc => qc.Value)
            .ToArray();


        private static string DefaultCategory = "default";
        private static bool DefaultSet = false;

        public static void Init(XVNMLModule module)
        {
            if (IsNull) return;

            Instance.module ??= module;

            var questDefinitions = module.Get<QuestDefinitions>();
            if (questDefinitions == null) return;

            var questCategories = questDefinitions.QuestCategoryMap;
            if (questCategories == null) return;

            QuestControl = new SortedDictionary<(string, string), QuestLog>();

            foreach(var category in questCategories)
            {
                if (category.Value.category.IsDefault && DefaultSet == false)
                {
                    DefaultSet = !DefaultSet;
                    DefaultCategory = category.Key;
                }

                CreateNewQuestLogCategory(category);
                Debug.Log($"Total Quests in {category.Key}: {category.Value.quests.Length}");
            }
        }

        public static void InitializeQuest(string questID, string? questCategory)
        {
            questCategory ??= DefaultCategory;
            QuestControl[(questCategory, questID)].Initialize();
        }

        public static void SetQuestActive(bool active, string questID, string? questCategory)
        {
            questCategory ??= DefaultCategory;
            QuestControl[(questCategory, questID)].SetActive(active);
        }

        public static void CompleteCurrentTask(string questID, string? questCategory)
        {
            questCategory ??= DefaultCategory;
            QuestControl[(questCategory, questID)].CompleteCurrentTask();
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
                QuestLog newLog = new QuestLog()
                {
                    questName = quest.TagName ?? quest.TagID.ToString(),
                    questDescription = quest.Description,
                };

                newLog.GenerateQuestTasks(quest.Objectives);

                newLog.onQuestInitialize += () => Instance._onQuestInitialize.Invoke();
                newLog.onQuestActive += () => Instance._onQuestActive.Invoke();
                newLog.onQuestInActive += () => Instance._onQuestInitialize.Invoke();
                newLog.onQuestComplete += () => Instance._onQuestComplete.Invoke();

                newLog.onNextTask += () => Instance._onNextTask.Invoke();
                newLog.onTaskComplete += () => Instance._onTaskComplete.Invoke();

                QuestControl.Add((categoryID, quest.TagName ?? quest.TagID.ToString()), newLog);
            }
        }
    }

    public sealed class QuestLog
    {
        public string questName;
        public string questDescription;

        public Action onQuestInitialize;
        public Action onQuestActive;
        public Action onQuestInActive;
        public Action onQuestComplete;
        
        public Action onNextTask;
        public Action onTaskComplete;
        
        public int taskID = -1;
        public SortedDictionary<string, bool> TaskLog;

        private Task[] _tasks;
        private bool _isActive;
        private bool _isComplete;

        public string TaskTitle => _currentTask.title;
        
        public bool Active
        {
            get
            {
                return _isActive;
            }
            private set
            {
                _isActive = value;
                Action activeStateAction = _isActive ? onQuestActive : onQuestInActive;
                activeStateAction.Invoke();
            }
        }

        public bool Complete
        {
            get
            {
                return _isComplete;
            }
            private set
            {
                _isComplete = value;
                if (_isComplete == false) return;
                onQuestComplete?.Invoke();
            }
        }

        private Task _currentTask => _tasks[taskID];

        public void SetActive(bool active) => Active = active;

        public void Initialize()
        {
            onQuestInitialize?.Invoke();
            Active = true;
            NextTask();
            onNextTask?.Invoke();
        }

        public void GenerateQuestTasks(TaskList objectives)
        {
            if (objectives == null) return;

            _tasks = objectives.Items;
            if (_tasks == null) return;

            TaskLog = new SortedDictionary<string, bool>();

            foreach ( var task in _tasks)
            {
                TaskLog.Add(task.TagName == "task" ? task.TagID.ToString() : task.TagName, false);
            }
        }

        public void CompleteCurrentTask()
        {
            var identifier = _currentTask.TagName ?? _currentTask.TagID.ToString();
            TaskLog[identifier] = true;

            onTaskComplete?.Invoke();
            
            NextTask();

            if (taskID > _tasks.Length - 1)
            {
                Complete = true;
                return;
            }

            onNextTask?.Invoke();
        }

        private void NextTask() => taskID++;
    }
}
