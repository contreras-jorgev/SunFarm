// Translated from AVR to C# on 9/28/2021 at 10:33:56 AM by ASNA Monarch Nomad® version 16.0.24.0
// ASNA Monarch(R) version 10.0.48.0 at 9/28/2021
// Migrated source location: library ERCAP, file QCLSRC, member MSGLOD

using ASNA.QSys.Runtime;
using ASNA.DataGate.Common;
using SunFarm.CustomerApp_Job;

using System;
using System.Collections;
using System.Collections.Specialized;
using ASNA.QSys.Runtime.JobSupport;




namespace YourCompany.YourApplication
{
    [ProgramEntry("_ENTRY")]
    public partial class Msglod : CLProgram
    {

        FixedString<_7> _MSGID;
        FixedString<_30> _MSGTXT;

        //------------------------------------------------------------------------------ 
        //  "*Entry" Mainline Code (Monarch generated)
        //------------------------------------------------------------------------------ 
        void StarEntry(int cparms)
        {
            _INLR = '1';


            if (!_MSGID.IsBlanks())
            {
                if (((string)_MSGID).Substring(0, 3) == "CST")
                    SendProgramMessage(_MSGID, "CUSTMSGF", _MSGTXT);
                else
                    SendProgramMessage(_MSGID, "ITEMMSGF", _MSGTXT);
            }


        }

#region Entry and activation methods for *ENTRY

        int _parms;

        void __ENTRY(ref FixedString<_7> __MSGID, ref FixedString<_30> __MSGTXT)
        {
            int cparms = 2;
            bool _cleanup = true;
            _MSGTXT = __MSGTXT;
            _MSGID = __MSGID;
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
                if (_cleanup)
                {
                    __MSGID = _MSGID;
                    __MSGTXT = _MSGTXT;
                }
            }
        }

        public static void _ENTRY(ICaller _caller, out Indicator __inLR, ref FixedString<_7> _MSGID, ref FixedString<_30> _MSGTXT)
        {
            FixedString<_7> _p_MSGID = _MSGID;
            FixedString<_30> _p_MSGTXT = _MSGTXT;
            __inLR = RunProgram<Msglod>(_caller, (Msglod _instance) => _instance.__ENTRY(ref _p_MSGID, ref _p_MSGTXT));
            _MSGID = _p_MSGID;
            _MSGTXT = _p_MSGTXT;
        }
#endregion
    }

}
