// Translated from AVR to C# on 9/28/2021 at 10:33:56 AM by ASNA Monarch Nomad® version 16.0.24.0
// ASNA Monarch(R) version 10.0.48.0 at 9/28/2021
using ASNA.QSys.Runtime;
using ASNA.DataGate.Common;
using System;
using SunFarm.CustomerApp;
using ASNA.QSys.Runtime.JobSupport;
namespace SunFarm.CustomerApp_Job
{

    public partial class MyJob : InteractiveJob
    {
        protected Indicator _INLR;
        protected Indicator _INRT;
        protected IndicatorArray<Len<_1, _0, _0>> _IN;
        protected dynamic DynamicCaller_;
        public Database MyDatabase = new Database("CYPRESS_5160");
        public Database MyPrinterDB = new Database("MonarchLocal");

        override protected Database getDatabase()
        {
            return MyDatabase;
        }

        override protected Database getPrinterDB()
        {
            return MyPrinterDB;
        }

        override public void Dispose(bool disposing)
        {
            if (disposing)
            {
                MyDatabase.Close();
                MyPrinterDB.Close();
            }
            base.Dispose(disposing);
        }

        MyJob(JobConfig jobConfig)
            : base(jobConfig)
        {
            _IN = new IndicatorArray<Len<_1, _0, _0>>((char[])null);
            _instanceInit();
        }

        public static MyJob JobFactory()
        {
            MyJob job = null;
            JobConfig jobConfig = new JobConfig();

            jobConfig.IFSRootPath = "//MyServer/MyShare";
            jobConfig.OutputQueueRootPath = "C:\\\\MonarchQueues\\\\OutputQueues";
            jobConfig.JobQueueBaseQueuesPath = "C:\\\\MonarchQueues\\\\JobQueues";

            job = new MyJob(jobConfig);
            return job;
        }

        override protected void ExecuteStartupProgram()
        {
            Indicator _LR = '0';
            MyDatabase.Open();
            MyPrinterDB.Open();

            DynamicCaller_.CallD("SunFarm.CustomerApp.Runci", out _LR);
        }


        void _instanceInit()
        {
            DynamicCaller_ = new DynamicCaller(this);
        }
    }

}
