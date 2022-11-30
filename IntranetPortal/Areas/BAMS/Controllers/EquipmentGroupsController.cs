using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IntranetPortal.Areas.BAMS.Models;
using IntranetPortal.Base.Models.AssetManagerModels;
using IntranetPortal.Base.Models.BamsModels;
using IntranetPortal.Base.Services;
using Microsoft.AspNetCore.Mvc;

namespace IntranetPortal.Areas.BAMS.Controllers
{
    [Area("BAMS")]
    public class EquipmentGroupsController : Controller
    {
        private readonly IBamsManagerService _bamsManagerService;
        private readonly IAssetManagerService _assetManagerService;

        public EquipmentGroupsController(IBamsManagerService bamsManagerService,
            IAssetManagerService assetManagerService)
        {
            _bamsManagerService = bamsManagerService;
            _assetManagerService = assetManagerService;
        }

        public async Task<IActionResult> Index()
        {
            EquipmentGroupsListViewModel model = new EquipmentGroupsListViewModel();
            try
            {
                var entities = await _bamsManagerService.GetAllEquipmentGroupsAsync();
                model.EquipmentGroupList = entities.ToList();
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }
            return View(model);
        }

        public IActionResult New()
        {
            EquipmentGroupsViewModel model = new EquipmentGroupsViewModel();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> New(EquipmentGroupsViewModel model)
        {
            if (ModelState.IsValid)
            {
                EquipmentGroup equipmentGroup = new EquipmentGroup();
                try
                {
                    if (model != null)
                    {
                        equipmentGroup = model.ConvertToEquipmentGroup();
                        equipmentGroup.CreatedTime = $"{DateTime.UtcNow.ToLongDateString()} {DateTime.UtcNow.ToLongTimeString()} + UTC";
                        equipmentGroup.CreatedBy = HttpContext.User.Identity.Name;

                        bool IsCreated = await _bamsManagerService.CreateEquipmentGroupAsync(equipmentGroup);
                        if (IsCreated)
                        {
                            model.OperationIsCompleted = true;
                            model.OperationIsSuccessful = true;
                            model.ViewModelSuccessMessage = "Equipment Group was created successfully!";
                        }
                        else
                        {
                            model.ViewModelErrorMessage = $"Sorry, an error was encountered. Creating Equipment Group failed.";
                        }
                    }
                }
                catch (Exception ex)
                {
                    model.ViewModelErrorMessage = ex.Message;
                }
            }
            else
            {
                model.ViewModelErrorMessage = $"Ooops! It appears some fields have missing or invalid values. Please correct this and try again.";
            }
            return View(model);
        }

        public async Task<IActionResult> Edit(int id)
        {
            EquipmentGroupsViewModel model = new EquipmentGroupsViewModel();
            if (id > 0)
            {
                EquipmentGroup equipmentGroup = await _bamsManagerService.GetEquipmentGroupByIdAsync(id);
                if (equipmentGroup != null)
                {
                    model.EquipmentGroupID = equipmentGroup.EquipmentGroupID;
                    model.EquipmentGroupName = equipmentGroup.EquipmentGroupName;
                    model.EquipmentGroupDescription = equipmentGroup.EquipmentGroupDescription;
                }
                else
                {
                    model.ViewModelErrorMessage = "Sorry, the record could not be found. Or it may have been deleted.";
                }
            }
            else
            {
                model.ViewModelErrorMessage = "Sorry, the record could not be retrieved. Key parameter has invalid value.";
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EquipmentGroupsViewModel model)
        {
            if (ModelState.IsValid)
            {
                EquipmentGroup equipmentGroup = new EquipmentGroup();
                try
                {
                    if (model != null)
                    {
                        equipmentGroup = model.ConvertToEquipmentGroup();
                        equipmentGroup.CreatedBy = HttpContext.User.Identity.Name;
                        equipmentGroup.CreatedTime = $"{DateTime.UtcNow.ToLongDateString()} {DateTime.UtcNow.ToLongTimeString()} + UTC";

                        bool IsCreated = await _bamsManagerService.UpdateEquipmentGroupAsync(equipmentGroup);
                        if (IsCreated)
                        {
                            model.OperationIsCompleted = true;
                            model.OperationIsSuccessful = true;
                            model.ViewModelSuccessMessage = $"Equipment Group updated successfully!";
                        }
                        else
                        {
                            model.ViewModelErrorMessage = $"Sorry, an error was encountered. Updating Equipment Group failed.";
                        }
                    }
                }
                catch (Exception ex)
                {
                    model.ViewModelErrorMessage = ex.Message;
                }
            }
            else
            {
                model.ViewModelErrorMessage = $"Ooops! It appears some fields have missing or invalid values. Please correct this and try again.";
            }
            return View(model);
        }

        public async Task<IActionResult> Delete(int id)
        {
            EquipmentGroupsViewModel model = new EquipmentGroupsViewModel();
            if (id > 0)
            {
                EquipmentGroup equipmentGroup = await _bamsManagerService.GetEquipmentGroupByIdAsync(id);
                if (equipmentGroup != null)
                {
                    model.EquipmentGroupID = equipmentGroup.EquipmentGroupID;
                    model.EquipmentGroupName = equipmentGroup.EquipmentGroupName;
                    model.EquipmentGroupDescription = equipmentGroup.EquipmentGroupDescription;
                }
                else
                {
                    model.ViewModelErrorMessage = "Sorry, the record could not be found. Or it may have been deleted.";
                }
            }
            else
            {
                model.ViewModelErrorMessage = "Sorry, the record could not be retrieved. Key parameter has invalid value.";
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(EquipmentGroupsViewModel model)
        {
            if (ModelState.IsValid)
            {
                EquipmentGroup equipmentGroup = new EquipmentGroup();
                try
                {
                    bool IsDeleted = await _bamsManagerService.DeleteEquipmentGroupAsync(model.EquipmentGroupID.Value);
                    if (IsDeleted)
                    {
                        return RedirectToAction("index");
                    }
                    else
                    {
                        model.ViewModelErrorMessage = $"Sorry, an error was encountered. Deleting Equipment Group failed.";
                    }
                }
                catch (Exception ex)
                {
                    model.ViewModelErrorMessage = ex.Message;
                }
            }
            else
            {
                model.ViewModelErrorMessage = $"Ooops! It appears some fields have missing or invalid values. Please correct this and try again.";
            }
            return View(model);
        }

        public async Task<IActionResult> EquipmentList(int id)
        {
            AssetEquipmentGroupViewModel model = new AssetEquipmentGroupViewModel();
            List<AssetEquipmentGroup> equipments = new List<AssetEquipmentGroup>();
            if (id > 0)
            {
                model.EquipmentGroupID = id;

                var entities = await _bamsManagerService.GetAssetEquipmentGroupsByEquipmentGroupIdAsync(id);
                equipments = entities.ToList();
            }

            ViewBag.EquipmentsList = equipments;
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EquipmentList(AssetEquipmentGroupViewModel model)
        {
            List<AssetEquipmentGroup> equipments = new List<AssetEquipmentGroup>();
            if (ModelState.IsValid)
            {
                try
                {
                    AssetEquipmentGroup assetEquipmentGroup = model.ConvertToAssetEquipmentGroup();
                    assetEquipmentGroup.AddedBy = HttpContext.User.Identity.Name;
                    assetEquipmentGroup.AddedTime = $"{DateTime.UtcNow.ToLongDateString()} {DateTime.UtcNow.ToLongTimeString()} + UTC";

                    Asset asset = await _assetManagerService.GetAssetByNameAsync(model.AssetName);
                    if (asset != null && !string.IsNullOrWhiteSpace(asset.AssetID))
                    {
                        assetEquipmentGroup.AssetID = asset.AssetID;
                        assetEquipmentGroup.AssetTypeID = asset.AssetTypeID;
                        assetEquipmentGroup.ConditionStatus = asset.ConditionStatus;
                        assetEquipmentGroup.CurrentLocation = asset.CurrentLocation;
                        string assetTypeName = asset.AssetTypeName;
                        var existing_entities = await _bamsManagerService.GetAssetEquipmentGroupsByEquipmentGroupIdAndEquipmentIdAsync(model.EquipmentGroupID, asset.AssetID);

                        if (existing_entities != null && existing_entities.Count > 0)
                        {
                            model.ViewModelWarningMessage = $"Sorry, this {assetTypeName} has already been added to this Group.";
                        }
                        else
                        {
                            bool IsAdded = await _bamsManagerService.CreateAssetEquipmentGroupAsync(assetEquipmentGroup);
                            if (IsAdded)
                            {
                                model.ViewModelSuccessMessage = "Equipment added to Group successfully!";
                            }
                        }
                    }
                    else
                    {
                        model.ViewModelErrorMessage = "Sorry, no record was found for the equipment you entered. Equipment was not added.";
                    }
                }
                catch (Exception ex)
                {
                    model.ViewModelErrorMessage = ex.Message;
                }
            }
            else
            {
                model.ViewModelErrorMessage = $"Ooops! It appears some fields have missing or invalid values. Please correct this and try again.";
            }
            var entities = await _bamsManagerService.GetAssetEquipmentGroupsByEquipmentGroupIdAsync(model.EquipmentGroupID);
            equipments = entities.ToList();
            ViewBag.EquipmentsList = equipments;
            return View(model);
        }

        [HttpPost]
        public async Task<string> Remove(int id)
        {
            if (id > 0)
            {
                bool IsRemoved = await _bamsManagerService.DeleteAssetEquipmentGroupAsync(id);
                if (IsRemoved)
                {
                    return "done";
                }
                else
                {
                    return "failed";
                }
            }
            else
            {
                return "none";
            }
        }
    }
}