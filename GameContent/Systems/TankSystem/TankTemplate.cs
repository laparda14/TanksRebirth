﻿using System;
using Microsoft.Xna.Framework;
using TanksRebirth.GameContent.Systems;
using TanksRebirth.GameContent.Systems.Coordinates;
using TanksRebirth.Internals.Common.Framework;

namespace TanksRebirth.GameContent;

public struct TankTemplate {
    /// <summary>If false, the template will contain data for an AI tank.</summary>
    public bool IsPlayer;

    public int AiTier;
    public int PlayerType;

    public Vector2 Position;

    private float _backingRotationField;

    public float Rotation { // Rounded to avoid issues when calculating rotation.
        get => _backingRotationField;
        set => _backingRotationField = MathF.Round(value, 5);
    }

    public int Team;

    public Range<int> RandomizeRange;

    public Tank GetTank() => IsPlayer ? GetPlayerTank() : GetAiTank();

    public AITank GetAiTank() {
        if (IsPlayer)
            throw new Exception($"{nameof(IsPlayer)} is true. This method cannot execute.");

        var ai = new AITank(AiTier);
        ai.Body.Position = Position / Tank.UNITS_PER_METER;
        ai.Position = Position;
        ai.TankRotation = Rotation;
        ai.TargetTankRotation = Rotation;
        ai.Dead = false;
        ai.TurretRotation = Rotation;
        ai.Team = Team;

        var placement = PlacementSquare.Placements.FindIndex(place => place.Position == ai.Position3D);
        if (placement > -1) {
            PlacementSquare.Placements[placement].TankId = ai.WorldId;
            PlacementSquare.Placements[placement].HasBlock = false;
        }

        return ai;
    }

    public PlayerTank GetPlayerTank() {
        if (!IsPlayer)
            throw new Exception($"{nameof(IsPlayer)} is false. This method cannot execute.");

        var player = Difficulties.Types["RandomPlayer"]
            ? new PlayerTank(PlayerType, false, AITank.PickRandomTier())
            : new PlayerTank(PlayerType);
        player.Body.Position = Position / Tank.UNITS_PER_METER;
        player.Position = Position;
        player.TankRotation = Rotation;
        player.Dead = false;
        player.Team = Team;

        var placement = PlacementSquare.Placements.FindIndex(place => place.Position == player.Position3D);
        if (placement > -1) {
            PlacementSquare.Placements[placement].TankId = player.WorldId;
            PlacementSquare.Placements[placement].HasBlock = false;
        }

        return player;
    }
}