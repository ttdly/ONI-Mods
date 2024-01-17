using Database;
using PeterHan.PLib.UI;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace PackAnything {
    public class MoveStatus {
        public bool HaveAnObjectMoving = false;
        public Surveyable surveyable;
        public WorldModifier worldModifier;
        public DelayMove delayMove;

        public MoveStatus() {
            surveyable = null;
            worldModifier = null;
            delayMove = null;
        }
    }

    public class PackAnythingStaticVars {
        public static ChoreType Survey;
        public static StatusItem SurveyingItem;
        public static StatusItem WaitingSurvey;
        public static List<Surveyable> SurveableCmps;
        public static Sprite ToolIcon;
        public static Color PrimaryColor;
        public static MoveStatus MoveStatus;
        public static HashSet<Tag> GeyserPerfab;

        public static void Init() {
            WaitingSurvey = Db.Get().MiscStatusItems.Add(
                new StatusItem("WaitingSurvey", "MISC", "status_item_needs_furniture", 
                StatusItem.IconType.Custom, NotificationType.Neutral, false, OverlayModes.None.ID));
            string[] chore_grops = new string[1] { nameof(ChoreTypes.Art) };
            Survey = (ChoreType)typeof(ChoreTypes)
                .GetMethod("Add", BindingFlags.Instance | BindingFlags.NonPublic)
                .Invoke(Db.Get().ChoreTypes, 
                new object[] { 
                    "Survey", 
                    chore_grops, 
                    "", 
                    new string[0], 
                    STRINGS.DUPLICANTS.CHORES.SURVEY.NAME.ToString(), 
                    STRINGS.DUPLICANTS.CHORES.SURVEY.STATUS.ToString(), 
                    STRINGS.DUPLICANTS.CHORES.SURVEY.TOOLTIP.ToString(), 
                    false, 
                    5000, 
                    STRINGS.DUPLICANTS.CHORES.SURVEY.REPORT_NAME.ToString() 
                });
            SurveableCmps = new List<Surveyable>();
            ToolIcon = PUIUtils.LoadSprite("PackAnything.images.tooIcon.png");
            PrimaryColor = new Color(0.5f, 0.7f, 1.0f, 1f);
            MoveStatus = new MoveStatus();
        }

        public static void SetMoving(bool isMoving) {
            MoveStatus.HaveAnObjectMoving = isMoving;
        }

        public static void SetTargetMove (DelayMove delayMove) {
            MoveStatus.delayMove = delayMove;
        }

        public static void SetTargetSurveyable(Surveyable surveyable) {
            MoveStatus.surveyable = surveyable;
        }

        public static void SetTargetModifier(WorldModifier worldModifier) {
            MoveStatus.worldModifier = worldModifier;
        }
    }
}
