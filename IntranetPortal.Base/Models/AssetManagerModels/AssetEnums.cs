using System.ComponentModel.DataAnnotations;

public enum AssetCondition
{ 
    [Display(Name="Select Condition")]
    Unspecified,
    [Display(Name="In Good Condition")]
    InGoodCondition,
    [Display(Name="Requires Repairs")]
    RequiresRepair,
    [Display(Name="Faulty Beyond Repair")]
    BeyondRepair
}