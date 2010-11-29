﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Squared.Task;
using System.Diagnostics;

namespace ShootBlues {
    public class SimpleExecutableProfile : DependencyManager, IProfile {
        public readonly string ExecutableName;
        protected ProcessWatcher Watcher;

        public SimpleExecutableProfile (string executableName) {
            ExecutableName = executableName;

            Name = new Filename(this.GetType().Assembly.Location).Name;

            Watcher = new ProcessWatcher(executableName);
        }

        public virtual string ProfileName {
            get {
                return ExecutableName;
            }
        }

        public virtual IEnumerator<object> Run () {
            while (Watcher != null) {
                var fNewProcess = Watcher.NewProcesses.Dequeue();
                yield return fNewProcess;

                yield return new Start(
                    OnNewProcess(fNewProcess.Result), TaskExecutionPolicy.RunAsBackgroundTask
                );
            }
        }

        protected virtual IEnumerator<object> OnNewProcess (Process process) {
            yield return Program.NotifyNewProcess(process);
        }

        public virtual void Dispose () {
            if (Watcher != null) {
                Watcher.Dispose();
                Watcher = null;
            }
        }
    }
}
