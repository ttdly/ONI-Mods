using PeterHan.PLib.Core;
using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace GravitasMemory {

    public static class Util {
        public static Texture2D DuplicateTexture(Texture2D source) {
            RenderTexture temporary = RenderTexture.GetTemporary(source.width, source.height, 0, RenderTextureFormat.Default, RenderTextureReadWrite.Linear);
            Graphics.Blit((Texture)source, temporary);
            RenderTexture active = RenderTexture.active;
            RenderTexture.active = temporary;
            Texture2D texture2D = new Texture2D(source.width, source.height);
            texture2D.ReadPixels(new Rect(0.0f, 0.0f, (float)temporary.width, (float)temporary.height), 0, 0);
            texture2D.Apply();
            RenderTexture.active = active;
            RenderTexture.ReleaseTemporary(temporary);
            return texture2D;
        }
    }

    public static class ElementUtil {
        public static void RegisterElementStrings(string elementId, string name, string description) {
            string upper = elementId.ToUpper();
            Strings.Add("STRINGS.ELEMENTS." + upper + ".NAME", UI.FormatAsLink(name, upper));
            Strings.Add("STRINGS.ELEMENTS." + upper + ".DESC", description);
        }

        public static KAnimFile FindAnim(string name) {
            KAnimFile anim1 = Assets.Anims.Find((Predicate<KAnimFile>)(anim => anim.name == name));
            if ((UnityEngine.Object)anim1 == (UnityEngine.Object)null)
                Debug.LogError((object)("Failed to find KAnim: " + name));
            return anim1;
        }

        public static void AddSubstance(Substance substance) => Assets.instance.substanceTable.GetList().Add(substance);

        public static Substance CreateSubstance(
          string name,
          Element.State state,
          KAnimFile kanim,
          Material material,
          Color32 colour) {
            return ModUtil.CreateSubstance(name, state, kanim, material, colour, colour, colour);
        }

        public static Substance CreateRegisteredSubstance(
          string name,
          Element.State state,
          KAnimFile kanim,
          Material material,
          Color32 colour) {
            Substance substance = ElementUtil.CreateSubstance(name, state, kanim, material, colour);
            SimHashUtil.RegisterSimHash(name);
            ElementUtil.AddSubstance(substance);
            PUtil.LogDebug("ElementLoader"+ substance.IsNullOrDestroyed() + substance.name);
            PUtil.LogDebug(ElementLoader.FindElementByName("Diamond").name);
            ElementLoader.FindElementByHash(substance.elementID).substance = substance;
            PUtil.LogDebug("ElementLoaderEnd");
            return substance;
        }
    }
    public static class SimHashUtil {
        public static Dictionary<SimHashes, string> SimHashNameLookup = new Dictionary<SimHashes, string>();
        public static readonly Dictionary<string, object> ReverseSimHashNameLookup = new Dictionary<string, object>();

        public static void RegisterSimHash(string name) {
            SimHashes key = (SimHashes)Hash.SDBMLower(name);
            SimHashUtil.SimHashNameLookup.Add(key, name);
            SimHashUtil.ReverseSimHashNameLookup.Add(name, (object)key);
        }
    }
}
