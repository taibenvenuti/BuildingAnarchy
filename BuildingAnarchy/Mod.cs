using ColossalFramework.UI;
using ICities;
using UnityEngine;
using static BuildingInfo;

namespace BuildingAnarchy
{
    public class Mod : LoadingExtensionBase, IUserMod
    {
        internal static string name = "Building Anarchy";
        public string Name => name;

        public string Description => "Plop buildings almost anywhere.";

        private BuildingAnarchy placementModeManager => BuildingAnarchy.instance;

        public override void OnLevelLoaded(LoadMode mode)
        {
            base.OnLevelLoaded(mode);
            placementModeManager.Initialize();
        }

        public override void OnLevelUnloading()
        {
            base.OnLevelUnloading();
            placementModeManager.Release();
        }

        #region UserInterface

        private static Settings settings;

        internal static Settings Settings
        {
            get
            {
                if (settings == null)
                {
                    settings = Settings.Load();
                    if (settings == null)
                    {
                        settings = new Settings();
                        settings.Save();
                    }
                }
                return settings;
            }
            set
            {
                settings = value;
            }
        }

        private UICheckBox useSavegameDataCheckbox;

        private UICheckBox saveGlobalDataOnDataChangedCheckbox;

        private UICheckBox displayPlacementModeCheckbox;

        private UICheckBox displayFlattenTerrainCheckbox;

        private UICheckBox displayFullGravelCheckbox;

        private UICheckBox displayFullPavementCheckbox;
        
        private static UIDropDown shortcutsDropdown;

        private UITextField keyboardShortcutsHelpTextField;

        private string[] shortcutOptions = new string[] { "UP and DOWN Arrow Keys", "K and L Keys" };

        private int selectedShortcutIndex => Settings.UseArrowKeys ? 0 : 1;

        public void OnSettingsUI(UIHelperBase helper)
        {
            helper.AddSpace(20);

            useSavegameDataCheckbox = (UICheckBox)helper.AddCheckbox("Save data to savegame file", Settings.UseSavegameData, (b) =>
            {
                Settings.UseSavegameData = b;
                Settings.Save();
                saveGlobalDataOnDataChangedCheckbox.isEnabled = !b;
                saveGlobalDataOnDataChangedCheckbox.opacity = b ? 0.25f : 1f;
            });

            useSavegameDataCheckbox.tooltip = 
                "Enable  this  option to save building settings on a per-city\n" +
                "basis.  Disabling  this option  makes all changes global ( ie.\n" +
                "buildings will load your customized settings on all cities.)";

            saveGlobalDataOnDataChangedCheckbox = (UICheckBox)helper.AddCheckbox("Save global data each time you modify a building's settings", Settings.SaveGlobalDataOnDataChanged, (b) =>
            {
                Settings.SaveGlobalDataOnDataChanged = b;

                Settings.Save();
            });


            saveGlobalDataOnDataChangedCheckbox.isEnabled = !Settings.UseSavegameData;

            saveGlobalDataOnDataChangedCheckbox.opacity = Settings.UseSavegameData ? 0.25f : 1f;

            saveGlobalDataOnDataChangedCheckbox.tooltip =
                "Enable this option to save data each time you modify a building's\n" +
                "settings.  This  option  only  works  when  using  Global  Data  (ie.\n" +
                "only  when  the  'Save data to  savegame file' option  is disabled.)";            
                        
            displayPlacementModeCheckbox = (UICheckBox)helper.AddCheckbox("Display Placement Mode in the construction cost tooltip", Settings.DisplayPlacementMode, (b) =>
            {
                Settings.DisplayPlacementMode = b;
                Settings.Save();
            });

            displayFlattenTerrainCheckbox = (UICheckBox)helper.AddCheckbox("Display Flatten Terrain enabled state in the construction cost tooltip", Settings.DisplayFlattenTerrain, (b) =>
            {
                Settings.DisplayFlattenTerrain = b;
                Settings.Save();
            });

            displayFullGravelCheckbox = (UICheckBox)helper.AddCheckbox("Display Full Gravel enabled state in the construction cost tooltip", Settings.DisplayFullGravel, (b) =>
            {
                Settings.DisplayFullGravel = b;
                Settings.Save();
            });            

            displayFullPavementCheckbox = (UICheckBox)helper.AddCheckbox("Display Full Pavement enabled state in the construction cost tooltip", Settings.DisplayFullPavement, (b) =>
            {
                Settings.DisplayFullPavement = b;
                Settings.Save();
            });
                

            helper.AddSpace(20);            

            shortcutsDropdown = (UIDropDown)helper.AddDropdown("Select Placement Mode keyboard shortcut preference", shortcutOptions, selectedShortcutIndex, (i) =>
            {
                Settings.UseArrowKeys = i == 0 ? true : false;

                Settings.Save();

                keyboardShortcutsHelpTextField.text = GenerateKeyboardShortcutText();
            });

            shortcutsDropdown.width = 480f;

            helper.AddSpace(20);

            keyboardShortcutsHelpTextField = (UITextField)helper.AddTextfield(" ", GenerateKeyboardShortcutText(), (s) => { }, (s) => { });

            keyboardShortcutsHelpTextField.size = new Vector2 (700f, 300f);

            keyboardShortcutsHelpTextField.Disable();
        }

        private string GenerateKeyboardShortcutText()
        {
            string text =
            "Keyboard shortcuts work only while building tool is active.\n\n" +
            $"{shortcutOptions[shortcutsDropdown.selectedIndex]} cycle through placement mode.\n\n" +
            "Press 'T' key to toggle Flatten Terrain.\n\n" +
            "Press 'G' key to toggle Full Gravel.\n\n" +
            "Press 'P' key to toggle Full Pavement.\n\n";
            return text;
        }
        #endregion
    }
}