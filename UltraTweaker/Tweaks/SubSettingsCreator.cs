using PluginConfig.API;
using PluginConfig.API.Fields;
using System;

namespace UltraTweaker.Tweaks
{
    internal static class SubSettingsCreator
    {
        public class IntSettingValues
        {
            public int Value;
            public readonly int Min;
            public readonly int Max;
            public readonly int Default;

            public IntSettingValues(int min, int max, int @default)
            {
                Value = @default;
                Min = min;
                Max = max;
                Default = @default;
            }
        }
        public static IntField CreateInt(
            Tweak tweak,
            string name,
            ConfigDivision division,
            Action<int> setter,
            int min,
            int max,
            int def)
        {
            IntField option = new IntField(
               division,
               name,
               $"{name}_{tweak.GetType().Name}_option",
               def,
               min,
               max);
            option.onValueChange += (IntField.IntValueChangeEvent e) =>
            {
                setter(e.value);
                tweak.OnSubsettingUpdate();
            };
            return option;
        }
        public static IntField CreateInt(
            Tweak tweak,
            string name,
            ConfigDivision division,
            IntSettingValues values)
        {
            IntField option = new IntField(
               division,
               name,
               $"{name}_{tweak.GetType().Name}_option",
               values.Default,
               values.Min,
               values.Max);
            option.onValueChange += (IntField.IntValueChangeEvent e) =>
            {
                values.Value = e.value;
                tweak.OnSubsettingUpdate();
            };
            return option;
        }

        public class FloatSettingValues
        {
            public float Value;
            public readonly float Min;
            public readonly float Max;
            public readonly float Default;

            public FloatSettingValues(float min, float max, float @default)
            {
                Value = @default;
                Min = min;
                Max = max;
                Default = @default;
            }
        }
        public static FloatField CreateFloat(
            Tweak tweak,
            string name,
            ConfigDivision division,
            Action<float> setter,
            float min,
            float max,
            float def)
        {
            FloatField option = new FloatField(
               division,
               name,
               $"{name}_{tweak.GetType().Name}_option",
               def,
               min,
               max);
            option.onValueChange += (FloatField.FloatValueChangeEvent e) =>
            {
                setter(e.value);
                tweak.OnSubsettingUpdate();
            };
            return option;
        }
        public static FloatField CreateFloat(
            Tweak tweak,
            string name,
            ConfigDivision division,
            FloatSettingValues values)
        {
            FloatField option = new FloatField(
               division,
               name,
               $"{name}_{tweak.GetType().Name}_option",
               values.Default,
               values.Min,
               values.Max);
            option.onValueChange += (FloatField.FloatValueChangeEvent e) =>
            {
                values.Value = e.value;
                tweak.OnSubsettingUpdate();
            };
            return option;
        }

        public class BoolSettingValues
        {
            public bool Value;
            public readonly bool Default;

            public BoolSettingValues(bool @default)
            {
                Value = @default;
                Default = @default;
            }
        }
        public static BoolField CreateBool(
            Tweak tweak,
            string name,
            ConfigDivision division,
            Action<bool> setter,
            bool def)
        {
            BoolField option = new BoolField(
               division,
               name,
               $"{name}_{tweak.GetType().Name}_option",
               def);
            option.onValueChange += (BoolField.BoolValueChangeEvent e) =>
            {
                setter(e.value);
                tweak.OnSubsettingUpdate();
            };
            return option;
        }
        public static BoolField CreateBool(
            Tweak tweak,
            string name,
            ConfigDivision division,
            BoolSettingValues values)
        {
            BoolField option = new BoolField(
               division,
               name,
               $"{name}_{tweak.GetType().Name}_option",
               values.Default);
            option.onValueChange += (BoolField.BoolValueChangeEvent e) =>
            {
                values.Value = e.value;
                tweak.OnSubsettingUpdate();
            };
            return option;
        }
    }
}
