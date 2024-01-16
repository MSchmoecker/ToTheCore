using System;
using System.Collections.Generic;
using System.Linq;
using BelowTheStone;
using BelowTheStone.LevelSystem;
using UnityEngine;

namespace ToTheCore {
    public class CoreRadarLogic : MonoBehaviour {
        private BasicItemLogic itemLogic;

        private void Awake() {
            itemLogic = GetComponent<BasicItemLogic>();
            itemLogic.RegisterCanUseItemHook(CheckItemUse);
            itemLogic.RegisterUseItemHook(OnUseItem);
        }

        private bool CheckItemUse(BasicItemLogic obj) {
            var level = LevelManager.CurrentLevel;

            if (level == null) {
                UI_ShortTextPresenter.DisplayText("Can only be used in the caves");
                return false;
            }

            return true;
        }

        private bool OnUseItem() {
            CameraShake.Shake(0.2f, 20f, 0.25f, CameraShake.ShakeInfo.Easing.sustained);

            var level = LevelManager.CurrentLevel;

            GameObject holderRoot = this.itemLogic.ItemBaseScript.GetHolderRoot();
            Vector2 itemPosition = holderRoot.transform.position;
            List<Vector2> exitPositions = new List<Vector2>();

            foreach (Chunk chunk in level.levelGrid.ChunkData) {
                if (chunk?.EntityList == null) {
                    continue;
                }

                foreach (EntityData entityData in chunk.EntityList) {
                    if (!entityData.EntityType || !entityData.EntityType.MainEntityPrefab) {
                        continue;
                    }

                    GameObject mainPrefab = entityData.EntityType.MainEntityPrefab;

                    if (mainPrefab.TryGetComponent(out BossCreature bossCreature)) {
                        exitPositions.Add(entityData.Position);
                        // Plugin.Log.LogInfo($"Boss: {bossCreature.name} at {entityData.Position}");
                    } else if (mainPrefab.TryGetComponent(out LayerExitPoint exitPoint)) {
                        exitPositions.Add(entityData.Position);
                        // Plugin.Log.LogInfo($"Exit: {exitPoint.name} at {entityData.Position}");
                    }
                }
            }

            if (exitPositions.Count == 0) {
                UI_ShortTextPresenter.DisplayText("No exits found!");
                return true;
            }

            Vector2 nearestExit = exitPositions.OrderBy(position => Vector2.Distance(position, itemPosition)).First();
            int distance = (int)Vector2.Distance(nearestExit, itemPosition);
            UI_ShortTextPresenter.DisplayText($"Nearest exit is {distance} block away!");

            return true;
        }
    }
}
