// Translated from AVR to C# on 9/28/2021 at 10:33:56 AM by ASNA Monarch Nomad® version 16.0.24.0
// ASNA Monarch(R) version 10.0.48.0 at 9/28/2021
// Migrated source location: library ERCAP, file QCLSRC, member MSGCLR

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
    public partial class Msgclr : CLProgram
    {


        //------------------------------------------------------------------------------ 
        //  "*Entry" Mainline Code (Monarch generated)
        //------------------------------------------------------------------------------ 
        void StarEntry(int cparms)
        {
            _INLR = '1';

            RemoveMessage("*ALL");
            return;


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
            __inLR = RunProgram<Msgclr>(_caller, (Msgclr _instance) => _instance.__ENTRY());
        }
#endregion
    }

}
