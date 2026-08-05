import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../core/localization/app_strings.dart';
import '../../core/models/stats_models.dart';
import '../../core/theme/app_theme.dart';
import '../../core/widgets/async_value_view.dart';
import 'stats_providers.dart';

class StatsScreen extends ConsumerWidget {
  const StatsScreen({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final strings = AppStrings.of(context);
    final stats = ref.watch(myStatsProvider);

    return Scaffold(
      appBar: AppBar(title: Text(strings.statsTitle, style: const TextStyle(fontWeight: FontWeight.w800))),
      body: AsyncValueView(
        value: stats,
        loadingHeight: 500,
        onRetry: () => ref.invalidate(myStatsProvider),
        data: (data) => ListView(
          padding: const EdgeInsets.all(16),
          children: [
            Row(
              children: [
                Expanded(child: _StatCard(label: strings.totalEpisodes, value: '${data.totalEpisodesWatched}', icon: Icons.live_tv)),
                const SizedBox(width: 12),
                Expanded(child: _StatCard(label: strings.totalMovies, value: '${data.totalMoviesWatched}', icon: Icons.movie)),
              ],
            ),
            const SizedBox(height: 12),
            _StatCard(
              label: strings.totalWatchTime,
              value: _formatMinutes(data.totalWatchTimeMinutes),
              icon: Icons.schedule,
              fullWidth: true,
            ),
            if (data.favoriteGenres.isNotEmpty) ...[
              const SizedBox(height: 24),
              Text(strings.favoriteGenres, style: Theme.of(context).textTheme.titleMedium?.copyWith(fontWeight: FontWeight.w700)),
              const SizedBox(height: 10),
              ...data.favoriteGenres.take(6).map((g) => _GenreBar(genre: g, max: data.favoriteGenres.first.count)),
            ],
            if (data.heatmapCalendar.isNotEmpty) ...[
              const SizedBox(height: 24),
              Text('Activity', style: Theme.of(context).textTheme.titleMedium?.copyWith(fontWeight: FontWeight.w700)),
              const SizedBox(height: 10),
              _Heatmap(days: data.heatmapCalendar),
            ],
            if (data.achievements.isNotEmpty) ...[
              const SizedBox(height: 24),
              Text(strings.achievements, style: Theme.of(context).textTheme.titleMedium?.copyWith(fontWeight: FontWeight.w700)),
              const SizedBox(height: 10),
              Wrap(
                spacing: 10,
                runSpacing: 10,
                children: data.achievements.map((badge) => _BadgeChip(badge: badge)).toList(),
              ),
            ],
            const SizedBox(height: 24),
          ],
        ),
      ),
    );
  }

  String _formatMinutes(int minutes) {
    final hours = minutes ~/ 60;
    final mins = minutes % 60;
    return '${hours}h ${mins}m';
  }
}

class _StatCard extends StatelessWidget {
  final String label;
  final String value;
  final IconData icon;
  final bool fullWidth;
  const _StatCard({required this.label, required this.value, required this.icon, this.fullWidth = false});

  @override
  Widget build(BuildContext context) {
    final scheme = Theme.of(context).colorScheme;
    return Card(
      child: Padding(
        padding: const EdgeInsets.all(16),
        child: Row(
          children: [
            Container(
              padding: const EdgeInsets.all(10),
              decoration: BoxDecoration(color: scheme.primaryContainer, borderRadius: BorderRadius.circular(AppTheme.radiusSm)),
              child: Icon(icon, color: scheme.onPrimaryContainer),
            ),
            const SizedBox(width: 12),
            Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(value, style: Theme.of(context).textTheme.titleLarge?.copyWith(fontWeight: FontWeight.w800)),
                Text(label, style: Theme.of(context).textTheme.bodySmall?.copyWith(color: scheme.onSurfaceVariant)),
              ],
            ),
          ],
        ),
      ),
    );
  }
}

class _GenreBar extends StatelessWidget {
  final GenreStat genre;
  final int max;
  const _GenreBar({required this.genre, required this.max});

  @override
  Widget build(BuildContext context) {
    final ratio = max == 0 ? 0.0 : genre.count / max;
    return Padding(
      padding: const EdgeInsets.only(bottom: 10),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Text(genre.genre, style: Theme.of(context).textTheme.bodyMedium),
          const SizedBox(height: 4),
          ClipRRect(
            borderRadius: BorderRadius.circular(6),
            child: LinearProgressIndicator(value: ratio, minHeight: 8),
          ),
        ],
      ),
    );
  }
}

class _Heatmap extends StatelessWidget {
  final List<HeatmapDay> days;
  const _Heatmap({required this.days});

  @override
  Widget build(BuildContext context) {
    final scheme = Theme.of(context).colorScheme;
    final maxCount = days.map((d) => d.count).fold(0, (a, b) => a > b ? a : b);
    final recent = days.length > 91 ? days.sublist(days.length - 91) : days;

    return Wrap(
      spacing: 4,
      runSpacing: 4,
      children: recent.map((day) {
        final intensity = maxCount == 0 ? 0.0 : day.count / maxCount;
        return Tooltip(
          message: '${day.date.toIso8601String().split('T').first}: ${day.count}',
          child: Container(
            width: 14,
            height: 14,
            decoration: BoxDecoration(
              color: Color.lerp(scheme.surfaceContainerHigh, scheme.primary, intensity.clamp(0.15, 1.0)),
              borderRadius: BorderRadius.circular(3),
            ),
          ),
        );
      }).toList(),
    );
  }
}

class _BadgeChip extends StatelessWidget {
  final AchievementBadge badge;
  const _BadgeChip({required this.badge});

  @override
  Widget build(BuildContext context) {
    final scheme = Theme.of(context).colorScheme;
    return Tooltip(
      message: badge.description,
      child: Container(
        padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 8),
        decoration: BoxDecoration(
          color: scheme.tertiaryContainer,
          borderRadius: BorderRadius.circular(AppTheme.radiusSm * 2),
        ),
        child: Row(
          mainAxisSize: MainAxisSize.min,
          children: [
            Icon(Icons.emoji_events, size: 16, color: scheme.onTertiaryContainer),
            const SizedBox(width: 6),
            Text(badge.name, style: TextStyle(color: scheme.onTertiaryContainer, fontWeight: FontWeight.w600)),
          ],
        ),
      ),
    );
  }
}
