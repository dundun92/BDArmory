using System.Collections;
using System.Collections.Generic;
using BDArmory.Modules;
using BDArmory.Core;
using UnityEngine;

namespace BDArmory.UI
{
	[KSPAddon(KSPAddon.Startup.Flight, false)]
	public class BDAUITeamIcons : MonoBehaviour
	{
		private List<MissileFire> _teamA;
		private List<MissileFire> _teamB;

		public BDAUITeamIcons Instance;
		private float updateList = 0;

		void Awake()
		{
			if (Instance)
			{
				Destroy(Instance);
			}

			Instance = this;
		}
		private void Start()
		{
			if (BDArmorySettings.DRAW_TEAM_ICONS)
			{
				UpdateList();
				GameEvents.onVesselCreate.Add(VesselEventUpdate);
				GameEvents.onVesselDestroy.Add(VesselEventUpdate);
				GameEvents.onVesselGoOffRails.Add(VesselEventUpdate);
				GameEvents.onVesselGoOnRails.Add(VesselEventUpdate);
			}
		}

		private void OnDestroy()
		{
			if (!BDArmorySettings.DRAW_TEAM_ICONS)
			{
				GameEvents.onVesselCreate.Remove(VesselEventUpdate);
				GameEvents.onVesselDestroy.Remove(VesselEventUpdate);
				GameEvents.onVesselGoOffRails.Remove(VesselEventUpdate);
				GameEvents.onVesselGoOnRails.Remove(VesselEventUpdate);
			}
		}

		private void MissileFireOnToggleTeam(MissileFire wm, BDArmorySetup.BDATeams team)
		{
			if (BDArmorySettings.DRAW_TEAM_ICONS)
			{
				UpdateList();
			}
		}

		private void VesselEventUpdate(Vessel v)
		{
			if (BDArmorySettings.DRAW_TEAM_ICONS)
			{
				UpdateList();
			}
		}
		private void Update()
		{
			if (BDArmorySettings.DRAW_TEAM_ICONS)
			{
				updateList -= Time.fixedDeltaTime;
				if (updateList < 0)
				{
					UpdateList();
					updateList = 0.5f;
				}
			}
		}
		private void UpdateList()
		{
			if (_teamA == null) _teamA = new List<MissileFire>();
			_teamA.Clear();

			if (_teamB == null) _teamB = new List<MissileFire>();
			_teamB.Clear();

			List<Vessel>.Enumerator v = FlightGlobals.Vessels.GetEnumerator();
			while (v.MoveNext())
			{
				if (v.Current == null) continue;
				if (!v.Current.loaded || v.Current.packed) continue;
				List<MissileFire>.Enumerator wm = v.Current.FindPartModulesImplementing<MissileFire>().GetEnumerator();
				while (wm.MoveNext())
				{
					if (wm.Current == null) continue;
					if (!wm.Current.team) _teamA.Add(wm.Current);
					else _teamB.Add(wm.Current);
					break;
				}
				wm.Dispose();
			}
			v.Dispose();
		}

		void OnGUI()
		{
			if (HighLogic.LoadedSceneIsFlight && BDArmorySetup.GAME_UI_ENABLED && !MapView.MapIsEnabled && BDArmorySettings.DRAW_TEAM_ICONS)
			{
				GUIStyle tAStyle = new GUIStyle();
				tAStyle.fontStyle = FontStyle.Bold;
				tAStyle.fontSize = 10;
				tAStyle.normal.textColor = Color.red;

				GUIStyle tBStyle = new GUIStyle();
				tBStyle.fontStyle = FontStyle.Bold;
				tBStyle.fontSize = 10;
				tBStyle.normal.textColor = XKCDColors.StrongBlue;

				{
					float size = 45;
					List<MissileFire>.Enumerator wma = _teamA.GetEnumerator();
					while (wma.MoveNext())
					{
						if (wma.Current == null) continue;
						if (!wma.Current.vessel.isActiveVessel)
						{
							Vector3 selfPos = FlightGlobals.ActiveVessel.CoM;
							Vector3 targetPos = (wma.Current.vessel.CoM);
							Vector3 targetRelPos = (targetPos - selfPos);
							Vector2 guiPos;
							float distance;
							string UIdist;
							string UoM;
							string vName;
							distance = targetRelPos.magnitude;
							if (distance >= 100)
							{
								if ((distance / 1000) >= 1)
								{
									UoM = "km";
									UIdist = (distance / 1000).ToString("0.00");
								}
								else
								{
									UoM = "m";
									UIdist = distance.ToString("0.0");
								}

								BDGUIUtils.DrawTextureOnWorldPos(wma.Current.vessel.CoM, BDArmorySetup.Instance.teamATexture, new Vector2(size, size), 0);
								if (BDGUIUtils.WorldToGUIPos(wma.Current.vessel.CoM, out guiPos))
								{
									if (BDArmorySettings.DRAW_TEAM_NAMES)
									{
										vName = wma.Current.vessel.vesselName;
										Rect nameRect = new Rect((guiPos.x + 24), guiPos.y - 4, 100, 32);
										GUI.Label(nameRect, vName, tAStyle);
									}
									Rect distRect = new Rect((guiPos.x - 12), (guiPos.y + 20), 100, 32);
									GUI.Label(distRect, UIdist + UoM, tAStyle);
								}
							}
						}
					}
					wma.Dispose();

					List<MissileFire>.Enumerator wmb = _teamB.GetEnumerator();
					while (wmb.MoveNext())
					{
						if (wmb.Current == null) continue;
						if (!wmb.Current.vessel.isActiveVessel)
						{
							Vector3 selfPos = FlightGlobals.ActiveVessel.CoM;
							Vector3 targetPos = (wmb.Current.vessel.CoM);
							Vector3 targetRelPos = (targetPos - selfPos);
							Vector2 guiPos;
							float distance;
							string UIdist;
							string UoM;
							string vName;
							distance = targetRelPos.magnitude;
							if (distance >= 100)
							{
								if ((distance / 1000) >= 1)
								{
									UoM = "km";
									UIdist = (distance / 1000).ToString("0.00");
								}
								else
								{
									UoM = "m";
									UIdist = distance.ToString("0.0");
								}
								BDGUIUtils.DrawTextureOnWorldPos(wmb.Current.vessel.CoM, BDArmorySetup.Instance.teamBTexture, new Vector2(size, size), 0);
								if (BDGUIUtils.WorldToGUIPos(wmb.Current.vessel.CoM, out guiPos))
								{
									if (BDArmorySettings.DRAW_TEAM_NAMES)
									{
										vName = wmb.Current.vessel.vesselName;
										Rect nameRect = new Rect((guiPos.x + 24), guiPos.y - 4, 100, 32);
										GUI.Label(nameRect, vName, tBStyle);
									}
									Rect distRect = new Rect((guiPos.x - 12), (guiPos.y + 20), 100, 32);
									GUI.Label(distRect, UIdist + UoM, tBStyle);
								}
							}
						}
					}
					wmb.Dispose();

					List<Vessel>.Enumerator v = FlightGlobals.Vessels.GetEnumerator();
					while (v.MoveNext())
					{
						if (v.Current == null) continue;
						if (v.Current.vesselType != VesselType.Debris && !v.Current.isActiveVessel) continue;
						{
							Vector3 sPos = FlightGlobals.ActiveVessel.vesselTransform.position;
							Vector3 tPos = v.Current.vesselTransform.position;
							Vector3 Dist = (tPos - sPos);
							if (Dist.magnitude > 100)
							{
								BDGUIUtils.DrawTextureOnWorldPos(v.Current.CoM, BDArmorySetup.Instance.debrisIconTexture, new Vector2(20, 20), 0);
							}
						}
					}
					v.Dispose();
				}
			}
		}
	}
}
