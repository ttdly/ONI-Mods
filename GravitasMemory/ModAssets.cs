using PeterHan.PLib.Core;
using PeterHan.PLib.UI;
using System;
using UnityEngine;

namespace GravitasMemory {
    public class ModAssets {
        public static void LoadStoryIcons() {
            try {
                Sprite GRAVITAS_MEMORY_ICON;
                Sprite GRAVITAS_MEMORY_IMAGE;
                Sprite GRAVITAS_MEMORY_CODEX;
                GRAVITAS_MEMORY_ICON = PUIUtils.LoadSprite("GravitasMemory.images.GravitasMemory_icon.png");
                GRAVITAS_MEMORY_ICON.name = GravitasMemory.CUSTOM.STORY_TRAITS.GRAVITAS_MEMORY.ICON_NAME;
                GRAVITAS_MEMORY_IMAGE = PUIUtils.LoadSprite("GravitasMemory.images.GravitasMemory_image.png");
                GRAVITAS_MEMORY_IMAGE.name = GravitasMemory.CUSTOM.STORY_TRAITS.GRAVITAS_MEMORY.IMAGE_NAME;
                GRAVITAS_MEMORY_CODEX = PUIUtils.LoadSprite("GravitasMemory.images.gravitas_memory_codex_icon.png");
                GRAVITAS_MEMORY_CODEX.name = GravitasMemory.CUSTOM.STORY_TRAITS.GRAVITAS_MEMORY.CDOEX_NAME;
                if (!Assets.Sprites.ContainsKey(GravitasMemory.CUSTOM.STORY_TRAITS.GRAVITAS_MEMORY.ICON_NAME)) {
                    Assets.Sprites.Add(GravitasMemory.CUSTOM.STORY_TRAITS.GRAVITAS_MEMORY.ICON_NAME, GRAVITAS_MEMORY_ICON);
                }
                if (!Assets.Sprites.ContainsKey(GravitasMemory.CUSTOM.STORY_TRAITS.GRAVITAS_MEMORY.IMAGE_NAME)) {
                    Assets.Sprites.Add(GravitasMemory.CUSTOM.STORY_TRAITS.GRAVITAS_MEMORY.IMAGE_NAME, GRAVITAS_MEMORY_IMAGE);
                }
                if (!Assets.Sprites.ContainsKey(GravitasMemory.CUSTOM.STORY_TRAITS.GRAVITAS_MEMORY.CDOEX_NAME)) {
                    Assets.Sprites.Add(GravitasMemory.CUSTOM.STORY_TRAITS.GRAVITAS_MEMORY.CDOEX_NAME, GRAVITAS_MEMORY_IMAGE);
                }
            }
            catch (ArgumentException e) {
                PUtil.LogException(e);
            }
        }
    }
}
