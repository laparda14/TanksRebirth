﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WiiPlayTanksRemake.Internals;
using WiiPlayTanksRemake.Internals.Common.Utilities;

namespace WiiPlayTanksRemake.GameContent
{
    public class Powerup
    {
        public const int MAX_POWERUPS = 50;
        public static Powerup[] activePowerups = new Powerup[MAX_POWERUPS];
        /// <summary>If this powerup is affecting a <see cref="Tank"/>, that <see cref="Tank"/> is this.</summary>
        public Tank AffectedTank { get; private set; }
        /// <summary>Whether or not this <see cref="Powerup"/> is affecting a <see cref="Tank"/>.</summary>
        public bool HasOwner => AffectedTank is not null;
        /// <summary>What this <see cref="Powerup"/> does to the <see cref="Tank"/> it affects.</summary>
        public Action<Tank> PowerupEffects { get; }
        /// <summary>The duration of this <see cref="Powerup"/> on a <see cref="Tank"/></summary>
        public int duration;

        public Vector3 position;

        /// <summary>The maximum distance from which a <see cref="Tank"/> can pick up this powerup.</summary>
        public float pickupRadius;

        public int id;

        /// <summary>Whether or not this powerup has been already picked up or not</summary>
        public bool InWorld { get; private set; }

        public Powerup(int duration, float pickupRadius, Action<Tank> effects)
        {
            this.pickupRadius = pickupRadius;
            this.duration = duration;

            PowerupEffects = effects;

            int index = Array.IndexOf(activePowerups, activePowerups.First(tank => tank is null));

            id = index;

            activePowerups[index] = this;
        }

        public Powerup(PowerupTemplate template)
        {
            pickupRadius = template.pickupRadius;
            duration = template.duration;
            PowerupEffects = template.PowerupEffects;

            int index = Array.IndexOf(activePowerups, activePowerups.First(tank => tank is null));

            id = index;

            activePowerups[index] = this;
        }

        /// <summary>Spawn this powerup in the world.</summary>
        public void Spawn(Vector3 position)
        {
            InWorld = true;

            this.position = position;
        }

        public void Update()
        {
            if (HasOwner)
            {
               //  AffectedTank.ApplyDefaults();
                // PowerupEffects?.Invoke(AffectedTank);
                duration--;
                if (duration <= 0)
                {
                    AffectedTank.ApplyDefaults();
                    activePowerups[id] = null;
                }
            }
            else
            {
                if (WPTR.AllTanks.TryGetFirst(tnk => tnk is not null && Vector3.Distance(position, tnk.position) <= pickupRadius, out Tank tank))
                {
                    Pickup(tank);
                }
            }
        }

        public void Render()
        {
            if (InWorld)
            {
                var pos = GeometryUtils.ConvertWorldToScreen(default, Matrix.CreateTranslation(position), TankGame.GameView, TankGame.GameProjection);

                DebugUtils.DrawDebugString(TankGame.spriteBatch, this, pos, 3, centerIt: true);

                TankGame.spriteBatch.Draw(GameResources.GetGameResource<Texture2D>("Assets/textures/WhitePixel"), GeometryUtils.CreateRectangleFromCenter((int)pos.X, (int)pos.Y, 25, 25), Color.White * 0.9f);
            }
            else
            {
                var pos = GeometryUtils.ConvertWorldToScreen(default, AffectedTank.World, TankGame.GameView, TankGame.GameProjection);

                DebugUtils.DrawDebugString(TankGame.spriteBatch, this, pos, 3, centerIt: true);
            }
        }
        /// <summary>
        /// Make a <see cref="Tank"/> pick this <see cref="Powerup"/> up.
        /// </summary>
        /// <param name="recipient">The recipient of this <see cref="Powerup"/>.</param>
        public void Pickup(Tank recipient)
        {
            AffectedTank = recipient;
            InWorld = false;

            PowerupEffects?.Invoke(AffectedTank);
        }
        public override string ToString()
        {
            if (AffectedTank is PlayerTank)
                return $"duration: {duration} | HasOwner: {HasOwner}" + (HasOwner ? $" | OwnerTier: {(AffectedTank as PlayerTank).PlayerType}" : "");
            else
                return $"duration: {duration} | HasOwner: {HasOwner}" + (HasOwner ? $" | OwnerTier: {(AffectedTank as AITank).tier}" : "");
        }
    }
    /// <summary>A template for creating powerups. The fields in this class are identical to the ones in <see cref="Powerup"/>.</summary>
    public struct PowerupTemplate
    {
        public float pickupRadius;
        public int duration;

        public Action<Tank> PowerupEffects { get; }

        public PowerupTemplate(int duration, float pickupRadius, Action<Tank> fx)
        {
            PowerupEffects = fx;

            this.pickupRadius = pickupRadius;
            this.duration = duration;
        }
    }
}
