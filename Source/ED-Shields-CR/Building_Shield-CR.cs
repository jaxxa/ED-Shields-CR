using Combat_Realism;
using EnhancedDevelopment.Shields.Basic;
using EnhancedDevelopment.Shields.Basic.ShieldUtils;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace ED_Shields_CR
{
    class Building_Shield_CR : EnhancedDevelopment.Shields.Basic.Building_Shield
    {

        /// <summary>
        /// Finds all projectiles at the position and destroys them
        /// </summary>
        /// <param name="square">The current square to protect</param>
        /// <param name="flag_direct">Block direct Fire</param>
        /// <param name="flag_indirect">Block indirect Fire</param>
        override public void ProtectSquare(IntVec3 square)
        {
            this.ProtectSquareCR(square);

            //Ignore squares outside the map
            if (!square.InBounds(this.Map))
            {
                return;
            }
            List<Thing> things = this.Map.thingGrid.ThingsListAt(square);
            List<Thing> thingsToDestroy = new List<Thing>();
            Boolean _IFFCheck = this.m_StructuralIntegrityMode;

            for (int i = 0, l = things.Count(); i < l; i++)
            {

                if (things[i] != null && things[i] is Projectile)
                {
                    //Assign to variable
                    Projectile pr = (Projectile)things[i];
                    if (!pr.Destroyed && ((this.m_BlockIndirect_Avalable && this.m_BlockDirect_Active && pr.def.projectile.flyOverhead) || (this.m_BlockDirect_Avalable && this.m_BlockIndirect_Active && !pr.def.projectile.flyOverhead)))
                    {
                        bool wantToIntercept = true;

                        //Check IFF
                        if (_IFFCheck == true)
                        {
                            //Log.Message("IFFcheck == true");
                            Thing launcher = ReflectionHelper.GetInstanceField(typeof(Projectile), pr, "launcher") as Thing;

                            if (launcher != null)
                            {
                                if (launcher.Faction != null)
                                {
                                    //Log.Message("launcher != null");
                                    if (launcher.Faction.IsPlayer)
                                    {
                                        wantToIntercept = false;
                                    }
                                    else
                                    {

                                    }
                                }
                            }
                        }

                        //Check OverShoot
                        if (pr.def.projectile.flyOverhead)
                        {
                            if (this.WillTargetLandInRange(pr))
                            {
                                //Log.Message("Fly Over");
                            }
                            else
                            {
                                wantToIntercept = false;
                                //Log.Message("In Range");
                            }
                        }


                        if (wantToIntercept)
                        {

                            //Detect proper collision using angles
                            Quaternion targetAngle = pr.ExactRotation;

                            Vector3 projectilePosition2D = pr.ExactPosition;
                            projectilePosition2D.y = 0;

                            Vector3 shieldPosition2D = Vectors.IntVecToVec(this.Position);
                            shieldPosition2D.y = 0;

                            Quaternion shieldProjAng = Quaternion.LookRotation(projectilePosition2D - shieldPosition2D);


                            if ((Quaternion.Angle(targetAngle, shieldProjAng) > 90))
                            {

                                //On hit effects
                                MoteMaker.ThrowLightningGlow(pr.ExactPosition, this.Map, 0.5f);
                                //On hit sound
                                HitSoundDef.PlayOneShot((SoundInfo)new TargetInfo(this.Position, this.Map, false));

                                //Damage the shield
                                ProcessDamage(pr.def.projectile.damageAmountBase);
                                //add projectile to the list of things to be destroyed
                                thingsToDestroy.Add(pr);
                            }
                        }
                        else
                        {
                            //Log.Message("Skip");
                        }

                    }
                }
            }
            foreach (Thing currentThing in thingsToDestroy)
            {
                currentThing.Destroy();
            }

        }


        public void ProtectSquareCR(IntVec3 square)
        {

            //Ignore squares outside the map
            if (!square.InBounds(this.Map))
            {
                return;
            }
            List<Thing> things = this.Map.thingGrid.ThingsListAt(square);
            List<Thing> thingsToDestroy = new List<Thing>();
            Boolean _IFFCheck = this.m_StructuralIntegrityMode;

            for (int i = 0, l = things.Count(); i < l; i++)
            {

                if (things[i] != null && things[i] is Combat_Realism.ProjectileCR)
                {
                    //Assign to variable
                    ProjectileCR pr = (ProjectileCR)things[i];
                    if (!pr.Destroyed && ((this.m_BlockIndirect_Avalable && this.m_BlockDirect_Active && pr.def.projectile.flyOverhead) || (this.m_BlockDirect_Avalable && this.m_BlockIndirect_Active && !pr.def.projectile.flyOverhead)))
                    {
                        bool wantToIntercept = true;

                        //Check IFF
                        if (_IFFCheck == true)
                        {
                            //Log.Message("IFFcheck == true");
                            Thing launcher = ReflectionHelper.GetInstanceField(typeof(ProjectileCR), pr, "launcher") as Thing;

                            if (launcher != null)
                            {
                                if (launcher.Faction != null)
                                {
                                    //Log.Message("launcher != null");
                                    if (launcher.Faction.IsPlayer)
                                    {
                                        wantToIntercept = false;
                                    }
                                    else
                                    {

                                    }
                                }
                            }
                        }

                        //Check OverShoot
                        if (pr.def.projectile.flyOverhead)
                        {
                            if (this.WillTargetLandInRangeCR(pr))
                            {
                                //Log.Message("Fly Over");
                            }
                            else
                            {
                                wantToIntercept = false;
                                //Log.Message("In Range");
                            }
                        }


                        if (wantToIntercept)
                        {

                            //Detect proper collision using angles
                            Quaternion targetAngle = pr.ExactRotation;

                            Vector3 projectilePosition2D = pr.ExactPosition;
                            projectilePosition2D.y = 0;

                            Vector3 shieldPosition2D = Vectors.IntVecToVec(this.Position);
                            shieldPosition2D.y = 0;

                            Quaternion shieldProjAng = Quaternion.LookRotation(projectilePosition2D - shieldPosition2D);


                            if ((Quaternion.Angle(targetAngle, shieldProjAng) > 90))
                            {

                                //On hit effects
                                MoteMaker.ThrowLightningGlow(pr.ExactPosition, this.Map, 0.5f);
                                //On hit sound
                                HitSoundDef.PlayOneShot((SoundInfo)new TargetInfo(this.Position, this.Map, false));

                                //Damage the shield
                                ProcessDamage(pr.def.projectile.damageAmountBase);
                                //add projectile to the list of things to be destroyed
                                thingsToDestroy.Add(pr);
                            }
                        }
                        else
                        {
                            //Log.Message("Skip");
                        }

                    }
                }
            }
            foreach (Thing currentThing in thingsToDestroy)
            {
                currentThing.Destroy();
            }

        }

        /// <summary>
        /// Checks if the projectile will land within the shield or pass over.
        /// </summary>
        /// <param name="projectile">The specific projectile that is being checked</param>
        /// <returns>True if the projectile will land close, false if it will be far away.</returns>
        public bool WillTargetLandInRangeCR(ProjectileCR projectile)
        {
            Vector3 targetLocation = GetTargetLocationFromProjectileCR(projectile);

            if (Vector3.Distance(this.Position.ToVector3(), targetLocation) > this.m_Field_Radius)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public Vector3 GetTargetLocationFromProjectileCR(ProjectileCR projectile)
        {
            FieldInfo fieldInfo = projectile.GetType().GetField("destination", BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
            Vector3 reoveredVector = (Vector3)fieldInfo.GetValue(projectile);
            return reoveredVector;
        }
    }
}
