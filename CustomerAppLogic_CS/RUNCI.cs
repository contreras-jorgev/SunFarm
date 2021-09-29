// Translated from AVR to C# on 9/28/2021 at 10:33:56 AM by ASNA Monarch Nomad® version 16.0.24.0
// ASNA Monarch(R) version 10.0.48.0 at 9/28/2021
// Migrated source location: library ERCAP, file QCLSRC, member RUNCI

using ASNA.QSys.Runtime;
using ASNA.DataGate.Common;
using SunFarm.CustomerApp_Job;

using System;
using System.Collections;
using System.Collections.Specialized;
using ASNA.QSys.Runtime.JobSupport;




namespace SunFarm.CustomerApp
{
    [ProgramEntry("_ENTRY")]
    public partial class Runci : CLProgram
    {
        protected dynamic DynamicCaller_;


        //------------------------------------------------------------------------------ 
        //  "*Entry" Mainline Code (Monarch generated)
        //------------------------------------------------------------------------------ 
        void StarEntry(int cparms)
        {
            Indicator _LR = '0';
            _INLR = '1';
            AddMsgToIgnore(typeof(CPF2103Exception)); /* LIBRARY SO_AND_SO ALREADY IN LIBL */
            // TODO : Warning: Please review commands that do not start with "Try" (.Net does not support Exception Catch/Ignore and Continue)


            TryAddLibLEntry("ERCAP");
            DynamicCaller_.CallD("SunFarm.CustomerApp.CUSTINQ", out _LR);
        }

#region Entry and activation methods for *ENTRY

        int _parms;

        void __ENTRY()
        {
            int cparms = 0;
            bool _cleanup = true;
            try
            {
                _parms = cparms;
                StarEntry(cparms);
            }
            catch(Return)
            {
            }
            catch(System.Threading.ThreadAbortException)
            {
                _cleanup = false;
                _INLR = '1';
            }
            finally
            {
                _ = _cleanup;
            }
        }

        public static void _ENTRY(ICaller _caller, out Indicator __inLR)
        {
            __inLR = RunProgram<Runci>(_caller, (Runci _instance) => _instance.__ENTRY());
        }
#endregion

        public Runci()
        {
            _instanceInit();
        }

        void _instanceInit()
        {
            DynamicCaller_ = new DynamicCaller(this);
        }
    }

}
