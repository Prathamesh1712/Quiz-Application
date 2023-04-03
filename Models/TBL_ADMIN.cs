

namespace QuizApp.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class TBL_ADMIN
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TBL_ADMIN()
        {
            this.TBL_CATEGORY = new HashSet<TBL_CATEGORY>();
        }
    
        public int AD_ID { get; set; }

        [Display(Name = "Username")]
        [Required(ErrorMessage = "*")]
        public string AD_NAME { get; set; }

        [Display(Name = "Password")]
        [Required(ErrorMessage = "*")]
        public string AD_PASSWORD { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TBL_CATEGORY> TBL_CATEGORY { get; set; }
    }
}
