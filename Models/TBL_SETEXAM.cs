//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace QuizApp.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class TBL_SETEXAM
    {
        public int EXAM_ID { get; set; }
        public Nullable<System.DateTime> EXAM_DATE { get; set; }
        public int EXAM_FK_STU { get; set; }
        public int EXAM_FK_CATID { get; set; }
        public Nullable<int> EXAM_STD_SCORE { get; set; }
    
        public virtual TBL_CATEGORY TBL_CATEGORY { get; set; }
        public virtual TBL_CATEGORY TBL_CATEGORY1 { get; set; }
        public virtual TBL_STUDENT TBL_STUDENT { get; set; }
    }
}
