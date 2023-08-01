using System;
using System.Collections.Generic;
using XVNML2U.Tags;

namespace XVNML2U.Data
{
    public sealed class QuestLog
    {
        public int taskID = -1;
        public string questName;
        public string questDescription;
        public string questCategoryName;

        public Action onQuestInitialize;
        public Action onQuestActive;
        public Action onQuestInActive;
        public Action onQuestComplete;
        
        public Action onNextTask;
        public Action onTaskComplete;
        
        public SortedDictionary<(int id, string title), bool> TaskLog;

        private Task[] _tasks;
        private bool _isActive;
        private bool _isComplete;

        public string TaskTitle => _currentTask.title;
        public int TaskLength => _tasks.Length;

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
            Active = true;
            NextTask();
            onQuestInitialize?.Invoke();
        }

        public void GenerateQuestTasks(TaskList objectives)
        {
            if (objectives == null) return;

            _tasks = objectives.Items;
            if (_tasks == null) return;

            TaskLog = new SortedDictionary<(int id, string title), bool>();

            foreach ( var task in _tasks)
            {
                TaskLog.Add((task.TagID.Value, task.title), false);
            }
        }

        public void CompleteCurrentTask()
        {
            int id = _currentTask.TagID.Value;
            var title = _currentTask.title;
            TaskLog[(id, title)] = true;

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
