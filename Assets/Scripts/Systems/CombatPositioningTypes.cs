using System;
using UnityEngine;

namespace JunkStorm.Systems
{
    public enum CombatPositionRole
    {
        Primary,
        Waiting
    }

    public enum CombatPositionState
    {
        Unassigned,
        Claimed,
        Occupying,
        InRange,
        Blocked,
        Released
    }

    public enum CombatPositionFallbackState
    {
        None,
        Holding,
        PassingLeft,
        PassingRight,
        Repathing,
        ReleasingClaim
    }

    public enum CombatPositionSide
    {
        None,
        Left,
        Right
    }

    public readonly struct CombatActorSnapshot
    {
        public readonly int ActorId;
        public readonly UnityEngine.Object Actor;
        public readonly Vector3 ActorPosition;
        public readonly float ActorRadius;
        public readonly UnityEngine.Object CurrentTarget;
        public readonly Vector3 TargetPosition;
        public readonly float TargetRadius;
        public readonly float LegalStopDistance;
        public readonly int RangeBand;
        public readonly int VerticalBand;
        public readonly bool CanApproach;
        public readonly bool HasMovementAuthority;
        public readonly bool IsAlive;
        public readonly bool IsActive;
        public readonly bool IsAttackCommitted;
        public readonly bool IsControlled;
        public readonly bool IsUnderExplicitOrForcedMovement;

        public CombatActorSnapshot(
            int actorId,
            UnityEngine.Object actor,
            Vector3 actorPosition,
            float actorRadius,
            UnityEngine.Object currentTarget,
            Vector3 targetPosition,
            float targetRadius,
            float legalStopDistance,
            int rangeBand,
            int verticalBand,
            bool canApproach,
            bool hasMovementAuthority,
            bool isAlive,
            bool isActive,
            bool isAttackCommitted,
            bool isControlled,
            bool isUnderExplicitOrForcedMovement)
        {
            ActorId = actorId;
            Actor = actor;
            ActorPosition = actorPosition;
            ActorRadius = actorRadius;
            CurrentTarget = currentTarget;
            TargetPosition = targetPosition;
            TargetRadius = targetRadius;
            LegalStopDistance = legalStopDistance;
            RangeBand = rangeBand;
            VerticalBand = verticalBand;
            CanApproach = canApproach;
            HasMovementAuthority = hasMovementAuthority;
            IsAlive = isAlive;
            IsActive = isActive;
            IsAttackCommitted = isAttackCommitted;
            IsControlled = isControlled;
            IsUnderExplicitOrForcedMovement = isUnderExplicitOrForcedMovement;
        }
    }

    public readonly struct CombatPositionGroupKey : IEquatable<CombatPositionGroupKey>
    {
        public readonly int TargetId;
        public readonly int VerticalBand;
        public readonly int RangeBand;
        public readonly int FactionOrCoordinatorId;

        public CombatPositionGroupKey(int targetId, int verticalBand, int rangeBand, int factionOrCoordinatorId)
        {
            TargetId = targetId;
            VerticalBand = verticalBand;
            RangeBand = rangeBand;
            FactionOrCoordinatorId = factionOrCoordinatorId;
        }

        public bool Equals(CombatPositionGroupKey other)
        {
            return TargetId == other.TargetId
                && VerticalBand == other.VerticalBand
                && RangeBand == other.RangeBand
                && FactionOrCoordinatorId == other.FactionOrCoordinatorId;
        }

        public override bool Equals(object obj)
        {
            return obj is CombatPositionGroupKey other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = TargetId;
                hashCode = (hashCode * 397) ^ VerticalBand;
                hashCode = (hashCode * 397) ^ RangeBand;
                hashCode = (hashCode * 397) ^ FactionOrCoordinatorId;
                return hashCode;
            }
        }

        public static bool operator ==(CombatPositionGroupKey left, CombatPositionGroupKey right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(CombatPositionGroupKey left, CombatPositionGroupKey right)
        {
            return !left.Equals(right);
        }
    }

    public struct CombatPositionClaim
    {
        public int ActorId;
        public CombatPositionGroupKey GroupKey;
        public int LayoutGeneration;
        public CombatPositionRole Role;
        public int RowIndex;
        public int SlotIndex;
        public CombatPositionState State;
        public bool IsTemporarilyBlocked;
        public CombatPositionFallbackState ProgressState;
    }

    public struct CombatPositionProgress
    {
        public Vector3 PreviousActorPosition;
        public Vector3 PreviousPreferredDestination;
        public float PreviousActorToDestinationDistance;
        public float TimeWithoutMeaningfulProgress;
        public float LastActualDisplacement;
        public CombatPositionFallbackState ActiveFallbackState;
        public CombatPositionSide PreferredPassingSide;
        public float SideSwitchCooldown;
        public float StateEntryTime;
    }

    public readonly struct CombatPositionResult
    {
        public readonly Vector3 PreferredWorldPosition;
        public readonly bool IsPrimary;
        public readonly bool MayEvaluateAttackLegality;
        public readonly float PhysicalPositioningRadius;
        public readonly float PositionTolerance;
        public readonly int LayoutGeneration;

        public CombatPositionResult(
            Vector3 preferredWorldPosition,
            bool isPrimary,
            bool mayEvaluateAttackLegality,
            float physicalPositioningRadius,
            float positionTolerance,
            int layoutGeneration)
        {
            PreferredWorldPosition = preferredWorldPosition;
            IsPrimary = isPrimary;
            MayEvaluateAttackLegality = mayEvaluateAttackLegality;
            PhysicalPositioningRadius = physicalPositioningRadius;
            PositionTolerance = positionTolerance;
            LayoutGeneration = layoutGeneration;
        }
    }

    [Serializable]
    public sealed class CombatConfig
    {
        public bool CombatPositioningEnabled = true;
        public float DefaultActorRadiusFallback = 0.35f;
        public float DefaultTargetRadiusFallback = 0.5f;
        public float PhysicalClearancePadding = 0.1f;
        public float PrimaryArcWidthDegrees = 180f;
        public int MaximumPrimarySlotsPerGroup = 6;
        public float WaitingRowSpacing = 0.75f;
        public float PositionTolerance = 0.12f;
        public float InRangeRepositionSpeedMultiplier = 0.6f;
        public float MaximumInRangeCorrectionDistance = 1.25f;
        public float RangeBandTolerance = 0.25f;
        public float MaximumVerticalGroupingDifference = 0.75f;
        public float MeaningfulRequestedMovementThreshold = 0.02f;
        public float MeaningfulAppliedDisplacementThreshold = 0.015f;
        public float BlockedDurationThreshold = 0.75f;
        public float FallbackMinimumHoldDuration = 0.25f;
        public float FallbackMaximumDuration = 2f;
        public float SideSwitchCooldown = 0.75f;
        public float TeleportResetDistance = 4f;
        public float TargetTeleportResetDistance = 4f;
        public float ResumeRangePadding = 0.15f;
        public bool DebugVisualizationEnabled;
        public bool RuntimeInvariantChecksEnabled = true;

        public void ValidateCombatPositioning()
        {
            DefaultActorRadiusFallback = SafePositive(DefaultActorRadiusFallback, 0.35f);
            DefaultTargetRadiusFallback = SafePositive(DefaultTargetRadiusFallback, 0.5f);
            PhysicalClearancePadding = SafeNonNegative(PhysicalClearancePadding, 0.1f);
            PrimaryArcWidthDegrees = ClampSafe(PrimaryArcWidthDegrees, 1f, 360f, 180f);
            MaximumPrimarySlotsPerGroup = Math.Max(0, MaximumPrimarySlotsPerGroup);
            WaitingRowSpacing = SafePositive(WaitingRowSpacing, 0.75f);
            PositionTolerance = SafePositive(PositionTolerance, 0.12f);
            InRangeRepositionSpeedMultiplier = ClampSafe(InRangeRepositionSpeedMultiplier, 0.05f, 1f, 0.6f);
            MaximumInRangeCorrectionDistance = SafePositive(MaximumInRangeCorrectionDistance, 1.25f);
            RangeBandTolerance = SafeNonNegative(RangeBandTolerance, 0.25f);
            MaximumVerticalGroupingDifference = SafePositive(MaximumVerticalGroupingDifference, 0.75f);
            MeaningfulRequestedMovementThreshold = SafePositive(MeaningfulRequestedMovementThreshold, 0.02f);
            MeaningfulAppliedDisplacementThreshold = SafePositive(MeaningfulAppliedDisplacementThreshold, 0.015f);
            BlockedDurationThreshold = SafePositive(BlockedDurationThreshold, 0.75f);
            FallbackMinimumHoldDuration = SafePositive(FallbackMinimumHoldDuration, 0.25f);
            FallbackMaximumDuration = SafePositive(FallbackMaximumDuration, 2f);
            SideSwitchCooldown = SafePositive(SideSwitchCooldown, 0.75f);
            TeleportResetDistance = SafePositive(TeleportResetDistance, 4f);
            TargetTeleportResetDistance = SafePositive(TargetTeleportResetDistance, 4f);
            ResumeRangePadding = SafeNonNegative(ResumeRangePadding, 0.15f);

            var usefulSlotSpacing = DefaultActorRadiusFallback + DefaultTargetRadiusFallback + PhysicalClearancePadding;
            if (PositionTolerance >= usefulSlotSpacing)
            {
                PositionTolerance = Math.Max(0.01f, usefulSlotSpacing * 0.5f);
            }

            var ordinaryFrameDisplacement = Math.Max(MeaningfulRequestedMovementThreshold, MeaningfulAppliedDisplacementThreshold) * 2f;
            if (TeleportResetDistance <= ordinaryFrameDisplacement)
            {
                TeleportResetDistance = Math.Max(4f, ordinaryFrameDisplacement * 10f);
            }

            if (TargetTeleportResetDistance <= ordinaryFrameDisplacement)
            {
                TargetTeleportResetDistance = Math.Max(4f, ordinaryFrameDisplacement * 10f);
            }

            if (FallbackMaximumDuration < FallbackMinimumHoldDuration)
            {
                FallbackMaximumDuration = FallbackMinimumHoldDuration;
            }
        }

        private static float SafePositive(float value, float fallback)
        {
            return IsFinite(value) && value > 0f ? value : fallback;
        }

        private static float SafeNonNegative(float value, float fallback)
        {
            return IsFinite(value) && value >= 0f ? value : fallback;
        }

        private static float ClampSafe(float value, float min, float max, float fallback)
        {
            if (!IsFinite(value))
            {
                return fallback;
            }

            if (value < min)
            {
                return min;
            }

            return value > max ? max : value;
        }

        private static bool IsFinite(float value)
        {
            return !float.IsNaN(value) && !float.IsInfinity(value);
        }
    }
}
