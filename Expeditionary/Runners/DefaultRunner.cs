﻿using Cardamom.Audio;
using Cardamom.Graphics;
using Cardamom.Ui;
using Expeditionary.View.Screens;

namespace Expeditionary.Runners
{
    public class DefaultRunner : UiRunner
    {
        public DefaultRunner(ProgramConfig config) 
            : base(config) { }

        protected override IRenderable MakeRoot(ProgramData data)
        {
            return MainMenuScreen.Create(new UiElementFactory(new AudioPlayer(), data.Resources));
        }
    }
}
