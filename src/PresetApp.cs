﻿using CommonAPI;
using CommonAPI.Phone;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using MovementFunk.SpeedDisplay;

namespace MovementFunk
{
    internal class PresetApp : CustomApp
    {
        private static Sprite IconSprite = null;
        private static List<string> presets;

        public static void Init()
        {
            string iconPath = Path.Combine(MovementFunkPlugin.Instance.Dir, "MF_icon.png");

            try
            {
                IconSprite = TextureUtility.LoadSprite(iconPath);
                PhoneAPI.RegisterApp<PresetApp>("MF Preset", IconSprite);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading icon sprite: {ex.Message}");
                PhoneAPI.RegisterApp<PresetApp>("MF Preset", null);
            }
        }

        public override void OnAppInit()
        {
            base.OnAppInit();
            CreateIconlessTitleBar("Available Presets");

            ScrollView = PhoneScrollView.Create(this);
            PopulateList();
        }

        private void PopulateList()
        {
            ScrollView.AddButton(NoneButton());
            presets = MFPresetManager.GetAvailablePresets();
            if (presets.Count > 0)
            {
                foreach (string preset in presets)
                {
                    Console.WriteLine(preset);
                    var button = CreatePresetButton(preset);
                    ScrollView.AddButton(button);
                }
            }
        }

        private static SimplePhoneButton CreatePresetButton(string preset)
        {
            var button = PhoneUIUtility.CreateSimpleButton(preset);
            button.OnConfirm += () =>
            {
                MFPresetManager.ApplyMovementPreset(preset);
                Speedometer.UpdateSpeedRep();
            };
            return button;
        }

        private static SimplePhoneButton NoneButton()
        {
            var button = PhoneUIUtility.CreateSimpleButton("None");
            button.OnConfirm += () =>
            {
                MFPresetManager.NoMovementPreset();
            };
            return button;
        }
    }
}
