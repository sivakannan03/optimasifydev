using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PfizerEntity
{
    public class ClsSettingMasterTeamwise
    {

        public string TeamFKID { get; set; }

        //PT
        public string PTcutOffDay { get; set; }

        //Cycle Plan
        public string CPDaysBeforeAllowed { get; set; }
        public string MinMonthlyFrequency { get; set; }
        public string MaxMothlyFrequency { get; set; }
        //CheckBox
        public string CPUnlockEntry { get; set; }

        //Tour Plan
        public string TPcutOffDays { get; set; }
        public string TPNoOfMonths { get; set; }
        //CheckBox
        public string TPlock { get; set; }

        //PSO Daily Report
        public string DREntryAllowedTime { get; set; }
        public string DREntryAllowedDays { get; set; }
        //CheckBox
        public string DRUnLockEntry { get; set; }

        //Doctor Profile...
        public string DPOLockPLStage { get; set; }

        //CheckBox
        public string DPPLOCKall { get; set; }

        //Cycle Plan Retailers RV
        public string CPRRVMinMonthlyFrequency { get; set; }
        public string CPRRVMaxMothlyFrequency { get; set; }

        //Cycle Plan Retailers RB
        public string CPRRBMinMonthlyFrequency { get; set; }
        public string CPRRBMaxMothlyFrequency { get; set; }

        //Product Pre Load...
        //CheckBox
        public string Product_Preloaded { get; set; }

        //DM Daily Report
        public string DM_DREntryAllowedTime { get; set; }
        public string DM_DREntryAllowedDays { get; set; }
        //checkbox
        public string DM_DRUnLockEntry { get; set; }

        //RBM Daily Report

        public string RBM_DREntryAllowedTime { get; set; }
        public string RBM_DREntryAllowedDays { get; set; }
        //CheckBox
        public string RBM_DRUnLockEntry { get; set; }

        //Secondary Sales  

        public string SS_MonthEnd { get; set; }
        public string SS_MonthBegin { get; set; }


    }
}
