using System;
using System.Collections.Generic;
using Windows.Devices.Enumeration;

namespace Angle_adjustment_Ex_for_MOVERIO_BT35E.Watchers
{
    internal abstract class MoverioWatcher
    {
        private readonly List<string> SupportMoverios = new List<string>()
        {
            "VID_0483&PID_5750",
            "VID_04B8&PID_0C0C"
        };

        private readonly DeviceWatcher deviceWatcher;
        public event EventHandler<string> Add;
        public event EventHandler<string> Remove;

        protected MoverioWatcher(string selector)
        {
            deviceWatcher = DeviceInformation.CreateWatcher(selector);
        }

        private void OnDeviceRemoved(DeviceWatcher sender, DeviceInformationUpdate args)
        {
            foreach (var id in SupportMoverios)
            {
                if (args.Id.Contains(id))
                {
                    OnRemove(args.Id);
                }
            }
        }

        private void OnDeviceAdded(DeviceWatcher sender, DeviceInformation args)
        {
            foreach (var id in SupportMoverios)
            {
                if (args.Id.Contains(id))
                {
                    OnAdd(args.Id);
                }
            }
        }

        public void Start()
        {
            if (deviceWatcher.Status != DeviceWatcherStatus.Started && deviceWatcher.Status != DeviceWatcherStatus.EnumerationCompleted)
            {
                deviceWatcher.Added += OnDeviceAdded;
                deviceWatcher.Removed += OnDeviceRemoved;
                deviceWatcher.Start();
            }
        }

        public void Stop()
        {
            if (deviceWatcher == null)
            {
                return;
            }

            switch (deviceWatcher.Status)
            {
                case DeviceWatcherStatus.Started:
                case DeviceWatcherStatus.EnumerationCompleted:
                    deviceWatcher.Added -= OnDeviceAdded;
                    deviceWatcher.Removed -= OnDeviceRemoved;
                    deviceWatcher.Stop();
                    break;
            }
        }

        protected virtual void OnAdd(string e)
        {
            Add?.Invoke(this, e);
        }

        protected virtual void OnRemove(string e)
        {
            Remove?.Invoke(this, e);
        }
    }
}