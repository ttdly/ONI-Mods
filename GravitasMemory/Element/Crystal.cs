using PeterHan.PLib.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GravitasMemory {
    public static class Crystal {
        public static readonly Color32 CRYSTAL_COLOR = new Color32((byte)201, (byte)201, (byte)195, byte.MaxValue);
        public const string SOLID_ID = "Crystal";
        public static readonly SimHashes SolidSimHash = (SimHashes)Hash.SDBMLower("Crystal");

        private static Texture2D TintTextureZincColor(Texture sourceTexture, string name) {
            Texture2D texture2D = Util.DuplicateTexture(sourceTexture as Texture2D);
            Color32[] pixels32 = texture2D.GetPixels32();
            for (int index = 0; index < pixels32.Length; ++index) {
                float num = ((Color)pixels32[index]).grayscale * 1.5f;
                pixels32[index] = (Color32)((Color)Crystal.CRYSTAL_COLOR * num);
            }
            texture2D.SetPixels32(pixels32);
            texture2D.Apply();
            texture2D.name = name;
            return texture2D;
        }

        private static Material CreateSolidZincMaterial(Material source) {
            Material solidZincMaterial = new Material(source);
            solidZincMaterial.mainTexture = (Texture)Crystal.TintTextureZincColor(solidZincMaterial.mainTexture, "crystal");
            solidZincMaterial.name = "matCrystal";
            return solidZincMaterial;
        }

        public static void RegisterCrystalSubstance() {
            Substance substance = Assets.instance.substanceTable.GetSubstance(SimHashes.Diamond);
            ElementUtil.CreateRegisteredSubstance("Crystal", Element.State.Solid, ElementUtil.FindAnim("crystal_kanim"), Crystal.CreateSolidZincMaterial(substance.material), Crystal.CRYSTAL_COLOR);
            PUtil.LogDebug("都完了");
        }
    }
}
