using JunkStorm.Systems;
using NUnit.Framework;

namespace JunkStorm.Tests.EditMode
{
    public sealed class CombatPositioningConfigTests
    {
        [Test]
        public void ValidateCombatPositioning_ReplacesInvalidNumericInputsWithSafeValues()
        {
            var config = new CombatConfig
            {
                DefaultActorRadiusFallback = float.NaN,
                DefaultTargetRadiusFallback = float.NegativeInfinity,
                PhysicalClearancePadding = float.PositiveInfinity,
                PrimaryArcWidthDegrees = float.NaN,
                WaitingRowSpacing = -1f,
                PositionTolerance = float.PositiveInfinity,
                InRangeRepositionSpeedMultiplier = float.NaN,
                MaximumInRangeCorrectionDistance = -1f,
                RangeBandTolerance = -1f,
                MaximumVerticalGroupingDifference = float.NegativeInfinity,
                MeaningfulRequestedMovementThreshold = -1f,
                MeaningfulAppliedDisplacementThreshold = float.NaN,
                BlockedDurationThreshold = 0f,
                FallbackMinimumHoldDuration = -1f,
                FallbackMaximumDuration = float.PositiveInfinity,
                SideSwitchCooldown = -1f,
                TeleportResetDistance = float.NaN,
                TargetTeleportResetDistance = -1f,
                ResumeRangePadding = float.NaN
            };

            config.ValidateCombatPositioning();

            Assert.That(config.DefaultActorRadiusFallback, Is.GreaterThan(0f));
            Assert.That(config.DefaultTargetRadiusFallback, Is.GreaterThan(0f));
            Assert.That(config.PhysicalClearancePadding, Is.GreaterThanOrEqualTo(0f));
            Assert.That(config.PrimaryArcWidthDegrees, Is.InRange(1f, 360f));
            Assert.That(config.WaitingRowSpacing, Is.GreaterThan(0f));
            Assert.That(config.PositionTolerance, Is.GreaterThan(0f));
            Assert.That(config.InRangeRepositionSpeedMultiplier, Is.InRange(0.05f, 1f));
            Assert.That(config.MaximumInRangeCorrectionDistance, Is.GreaterThan(0f));
            Assert.That(config.RangeBandTolerance, Is.GreaterThanOrEqualTo(0f));
            Assert.That(config.MaximumVerticalGroupingDifference, Is.GreaterThan(0f));
            Assert.That(config.MeaningfulRequestedMovementThreshold, Is.GreaterThan(0f));
            Assert.That(config.MeaningfulAppliedDisplacementThreshold, Is.GreaterThan(0f));
            Assert.That(config.BlockedDurationThreshold, Is.GreaterThan(0f));
            Assert.That(config.FallbackMinimumHoldDuration, Is.GreaterThan(0f));
            Assert.That(config.FallbackMaximumDuration, Is.GreaterThan(0f));
            Assert.That(config.SideSwitchCooldown, Is.GreaterThan(0f));
            Assert.That(config.TeleportResetDistance, Is.GreaterThan(0f));
            Assert.That(config.TargetTeleportResetDistance, Is.GreaterThan(0f));
            Assert.That(config.ResumeRangePadding, Is.GreaterThanOrEqualTo(0f));
        }

        [Test]
        public void ValidateCombatPositioning_ClampsArcSlotsAndSpeedMultiplier()
        {
            var config = new CombatConfig
            {
                PrimaryArcWidthDegrees = 999f,
                MaximumPrimarySlotsPerGroup = -3,
                InRangeRepositionSpeedMultiplier = 12f
            };

            config.ValidateCombatPositioning();

            Assert.That(config.PrimaryArcWidthDegrees, Is.EqualTo(360f));
            Assert.That(config.MaximumPrimarySlotsPerGroup, Is.EqualTo(0));
            Assert.That(config.InRangeRepositionSpeedMultiplier, Is.EqualTo(1f));
        }

        [Test]
        public void ValidateCombatPositioning_KeepsToleranceBelowUsefulSlotSpacing()
        {
            var config = new CombatConfig
            {
                DefaultActorRadiusFallback = 0.5f,
                DefaultTargetRadiusFallback = 0.5f,
                PhysicalClearancePadding = 0.2f,
                PositionTolerance = 3f
            };

            config.ValidateCombatPositioning();

            Assert.That(config.PositionTolerance, Is.LessThan(1.2f));
        }

        [Test]
        public void ValidateCombatPositioning_EnsuresTeleportThresholdsExceedOrdinaryFrameDisplacement()
        {
            var config = new CombatConfig
            {
                MeaningfulRequestedMovementThreshold = 0.5f,
                MeaningfulAppliedDisplacementThreshold = 0.25f,
                TeleportResetDistance = 0.1f,
                TargetTeleportResetDistance = 0.1f
            };

            config.ValidateCombatPositioning();

            Assert.That(config.TeleportResetDistance, Is.GreaterThan(1f));
            Assert.That(config.TargetTeleportResetDistance, Is.GreaterThan(1f));
        }

        [Test]
        public void ValidateCombatPositioning_EnsuresFallbackMaximumCoversMinimumHoldDuration()
        {
            var config = new CombatConfig
            {
                FallbackMinimumHoldDuration = 3f,
                FallbackMaximumDuration = 1f
            };

            config.ValidateCombatPositioning();

            Assert.That(config.FallbackMaximumDuration, Is.EqualTo(config.FallbackMinimumHoldDuration));
        }
    }
}
