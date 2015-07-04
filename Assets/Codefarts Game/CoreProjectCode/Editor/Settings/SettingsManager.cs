/*
<copyright>
  Copyright (c) 2012 Codefarts
  All rights reserved.
  contact@codefarts.com
  http://www.codefarts.com
</copyright>
*/
namespace Codefarts.CoreProjectCode.Settings
{
    using System;

    using Codefarts.Localization;

    public sealed class SettingsManager : IValues<string>
    {
        private static SettingsManager settingsManager;

        public event EventHandler<ValueChangedEventArgs<string>> ValueChanged;

        private IValues<string> values;

        private readonly ValueChangedEventArgs<string> eventArgs = new ValueChangedEventArgs<string>();

        public static SettingsManager Instance
        {
            get
            {
                return settingsManager ?? (settingsManager = new SettingsManager());
            }
        }

        public IValues<string> Values
        {
            get
            {
                return this.values;
            }

            set
            {
                if (this.values != null)
                {
                    this.values.ValueChanged -= this.ValuesValueChanged;
                }

                this.values = value;

                if (this.values != null)
                {
                    this.values.ValueChanged += this.ValuesValueChanged;
                }
            }
        }

        void ValuesValueChanged(object sender, ValueChangedEventArgs<string> e)
        {
            this.DoSettingChanged(e.Name, e.Value);
        }

        public T GetValue<T>(string key)
        {
            if (this.values == null)
            {
                var local = LocalizationManager.Instance;
                throw new NullReferenceException(local.Get("ERR_SettingsPropNotSet"));
            }

            return this.values.GetValue<T>(key);
        }

        public void RemoveValue(string name)
        {
            this.values.RemoveValue(name);
        }

        public void SetValue(string name, object value)
        {
            if (this.values == null)
            {
                var local = LocalizationManager.Instance;
                throw new NullReferenceException(local.Get("ERR_SettingsPropNotSet"));
            }

            this.values.SetValue(name, value);
            this.DoSettingChanged(name, value);
        }

        private void DoSettingChanged(string name, object value)
        {
            if (this.ValueChanged != null)
            {
                this.eventArgs.Name = name;
                this.eventArgs.Value = value;
                this.ValueChanged(this, this.eventArgs);
                this.eventArgs.Value = null;
            }
        }

        public bool HasValue(string name)
        {
            if (this.values == null)
            {
                return false;
            }

            return this.values.HasValue(name);
        }

        public string[] GetValueKeys()
        {
            return this.values.GetValueKeys();
        }
    }
}