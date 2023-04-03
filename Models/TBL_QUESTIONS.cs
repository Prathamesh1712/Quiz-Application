

namespace QuizApp.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class TBL_QUESTIONS
    {
        public int QUESTION_ID { get; set; }

        [Display(Name = "Question")]
        [Required(ErrorMessage = "*")]
        public string Q_TEXT { get; set; }

        [Display(Name = "Option A")]
        [Required(ErrorMessage = "*")]
        public string OPA { get; set; }

        [Display(Name = "Option B")]
        [Required(ErrorMessage = "*")]
        public string OPB { get; set; }

        [Display(Name = "Option C")]
        [Required(ErrorMessage = "*")]
        public string OPC { get; set; }

        [Display(Name = "Option D")]
        [Required(ErrorMessage = "*")]
        public string OPD { get; set; }

        [Display(Name = "Correct Option")]
        [Required(ErrorMessage = "*")]
        public string COP { get; set; }
        public Nullable<int> Q_FK_CATID { get; set; }
    
        public virtual TBL_CATEGORY TBL_CATEGORY { get; set; }
    }
}
