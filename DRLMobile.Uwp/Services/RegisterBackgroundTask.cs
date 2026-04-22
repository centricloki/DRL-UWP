using DRLMobile.ExceptionHandler;
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;

namespace DRLMobile.Uwp.Services
{
    public static class RegisterBackgroundTask
    {

        public static async Task<BackgroundTaskRegistration> Register(string entryPoint, string taskName, IBackgroundTrigger trigger, IBackgroundCondition backgroundCondition, bool isBackgroundAccess)
        {
            foreach (var backgroundTask in BackgroundTaskRegistration.AllTasks)
            {
                if (backgroundTask.Value.Name.Equals(taskName))
                    return (BackgroundTaskRegistration)backgroundTask.Value;
            }

            if (isBackgroundAccess)
            {
                var isAccessGranted = await BackgroundExecutionManager.RequestAccessAsync();

            }
            var taskBuilder = new BackgroundTaskBuilder();

            taskBuilder.Name = taskName;
            taskBuilder.TaskEntryPoint = entryPoint;
            taskBuilder.SetTrigger(trigger);


            if (backgroundCondition != null)
            {
                taskBuilder.AddCondition(backgroundCondition);
                taskBuilder.CancelOnConditionLoss = true;
            }
            BackgroundTaskRegistration task = taskBuilder.Register();
            return task;
        }

        public static void UnRegister(string name)
        {
            try
            {
                foreach (var item in BackgroundTaskRegistration.AllTasks)
                {
                    if (item.Value.Name.Equals(name))
                    {
                        item.Value.Unregister(true);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.LogAndThrowSpecifiedException(nameof(RegisterBackgroundTask), nameof(UnRegister), ex);
            }
        }
    }
}
