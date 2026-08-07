import 'package:cached_network_image/cached_network_image.dart';
import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../core/config/tmdb_images.dart';
import '../../core/localization/app_strings.dart';
import '../../core/models/engagement_models.dart';
import '../../core/models/tracking_models.dart';
import '../../core/network/api_exception.dart';
import '../../core/providers/core_providers.dart';
import '../../core/theme/app_theme.dart';
import '../../core/widgets/async_value_view.dart';
import '../engagement/engagement_section.dart';
import 'episode_providers.dart';

class SeasonEpisodesScreen extends ConsumerWidget {
  final int seriesTmdbId;
  final int seasonNumber;
  const SeasonEpisodesScreen({
    super.key,
    required this.seriesTmdbId,
    required this.seasonNumber,
  });

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final strings = AppStrings.of(context);
    final key = (seriesTmdbId: seriesTmdbId, seasonNumber: seasonNumber);
    final data = ref.watch(seasonWithProgressProvider(key));

    return Scaffold(
      appBar: AppBar(title: Text('${strings.seasons} $seasonNumber')),
      body: AsyncValueView(
        value: data,
        loadingHeight: 400,
        onRetry: () => ref.invalidate(seasonWithProgressProvider(key)),
        data: (season) => Column(
          crossAxisAlignment: CrossAxisAlignment.stretch,
          children: [
            Padding(
              padding: const EdgeInsets.fromLTRB(16, 12, 16, 4),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(
                    '${season.progress.watchedEpisodes} / ${season.progress.totalEpisodes} watched',
                    style: Theme.of(context).textTheme.bodyMedium,
                  ),
                  const SizedBox(height: 6),
                  ClipRRect(
                    borderRadius: BorderRadius.circular(8),
                    child: LinearProgressIndicator(
                      value: season.progress.totalEpisodes == 0
                          ? 0
                          : season.progress.watchedEpisodes /
                                season.progress.totalEpisodes,
                      minHeight: 8,
                    ),
                  ),
                ],
              ),
            ),
            Expanded(
              child: ListView.builder(
                padding: const EdgeInsets.symmetric(vertical: 8),
                itemCount: season.rows.length,
                itemBuilder: (context, index) => _EpisodeTile(
                  seriesTmdbId: seriesTmdbId,
                  seasonNumber: seasonNumber,
                  row: season.rows[index],
                ),
              ),
            ),
          ],
        ),
      ),
    );
  }
}

class _EpisodeTile extends ConsumerStatefulWidget {
  final int seriesTmdbId;
  final int seasonNumber;
  final EpisodeRow row;
  const _EpisodeTile({
    required this.seriesTmdbId,
    required this.seasonNumber,
    required this.row,
  });

  @override
  ConsumerState<_EpisodeTile> createState() => _EpisodeTileState();
}

class _EpisodeTileState extends ConsumerState<_EpisodeTile> {
  bool _isBusy = false;

  Future<void> _setStatus(EpisodeWatchStatus status) async {
    setState(() => _isBusy = true);
    try {
      await ref
          .read(trackingApiProvider)
          .markEpisode(
            seriesTmdbId: widget.seriesTmdbId,
            seasonNumber: widget.seasonNumber,
            episodeNumber: widget.row.summary.episodeNumber,
            status: status,
          );
      final key = (
        seriesTmdbId: widget.seriesTmdbId,
        seasonNumber: widget.seasonNumber,
      );
      ref.invalidate(seasonWithProgressProvider(key));
    } catch (e) {
      if (mounted) {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(
            content: Text(e is ApiException ? e.message : 'Failed to update.'),
          ),
        );
      }
    } finally {
      if (mounted) setState(() => _isBusy = false);
    }
  }

  Future<void> _toggleFavorite() async {
    try {
      await ref
          .read(trackingApiProvider)
          .toggleEpisodeFavorite(
            seriesTmdbId: widget.seriesTmdbId,
            seasonNumber: widget.seasonNumber,
            episodeNumber: widget.row.summary.episodeNumber,
            isFavorite: !widget.row.progress.isFavorite,
          );
      final key = (
        seriesTmdbId: widget.seriesTmdbId,
        seasonNumber: widget.seasonNumber,
      );
      ref.invalidate(seasonWithProgressProvider(key));
    } catch (_) {}
  }

  Future<void> _openDiscussion() async {
    final summary = widget.row.summary;
    final strings = AppStrings.of(context);

    await showModalBottomSheet<void>(
      context: context,
      isScrollControlled: true,
      showDragHandle: true,
      builder: (sheetContext) => SafeArea(
        child: Padding(
          padding: EdgeInsets.fromLTRB(
            16,
            0,
            16,
            16 + MediaQuery.of(sheetContext).viewInsets.bottom,
          ),
          child: SingleChildScrollView(
            child: Column(
              mainAxisSize: MainAxisSize.min,
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(
                  '${summary.episodeNumber}. ${summary.title}',
                  style: Theme.of(
                    sheetContext,
                  ).textTheme.titleLarge?.copyWith(fontWeight: FontWeight.w800),
                ),
                if (summary.overview != null &&
                    summary.overview!.isNotEmpty) ...[
                  const SizedBox(height: 8),
                  Text(
                    summary.overview!,
                    style: Theme.of(sheetContext).textTheme.bodyMedium,
                  ),
                ],
                const SizedBox(height: 16),
                EngagementSection(
                  targetType: TargetType.episode,
                  targetId: summary.id,
                  title: strings.episodeDiscussion,
                  commentHint: '${strings.shareThoughts} ${summary.title}',
                  compact: true,
                ),
              ],
            ),
          ),
        ),
      ),
    );
  }

  @override
  Widget build(BuildContext context) {
    final summary = widget.row.summary;
    final progress = widget.row.progress;
    final scheme = Theme.of(context).colorScheme;
    final strings = AppStrings.of(context);

    return Card(
      margin: const EdgeInsets.symmetric(horizontal: 16, vertical: 6),
      child: Padding(
        padding: const EdgeInsets.all(14),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Row(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                ClipRRect(
                  borderRadius: BorderRadius.circular(AppTheme.radiusSm),
                  child: SizedBox(
                    width: 110,
                    height: 62,
                    child: summary.stillPath != null
                        ? CachedNetworkImage(
                            imageUrl: TmdbImages.still(summary.stillPath)!,
                            fit: BoxFit.cover,
                          )
                        : Container(
                            color: scheme.surfaceContainerHighest,
                            child: const Icon(Icons.tv),
                          ),
                  ),
                ),
                const SizedBox(width: 12),
                Expanded(
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Text(
                        '${summary.episodeNumber}. ${summary.title}',
                        style: Theme.of(context).textTheme.bodyLarge?.copyWith(
                          fontWeight: FontWeight.w700,
                        ),
                      ),
                      if (summary.airDate != null) ...[
                        const SizedBox(height: 4),
                        Text(
                          '${summary.airDate!.year}-${summary.airDate!.month.toString().padLeft(2, '0')}-${summary.airDate!.day.toString().padLeft(2, '0')}',
                          style: Theme.of(context).textTheme.bodySmall
                              ?.copyWith(color: scheme.onSurfaceVariant),
                        ),
                      ],
                      if (summary.overview != null &&
                          summary.overview!.isNotEmpty) ...[
                        const SizedBox(height: 8),
                        Text(
                          summary.overview!,
                          maxLines: 3,
                          overflow: TextOverflow.ellipsis,
                          style: Theme.of(context).textTheme.bodySmall
                              ?.copyWith(color: scheme.onSurfaceVariant),
                        ),
                      ],
                    ],
                  ),
                ),
              ],
            ),
            const SizedBox(height: 12),
            Row(
              children: [
                TextButton.icon(
                  onPressed: _openDiscussion,
                  icon: const Icon(Icons.forum_outlined),
                  label: Text(strings.community),
                ),
                const Spacer(),
                if (_isBusy)
                  const SizedBox(
                    width: 20,
                    height: 20,
                    child: CircularProgressIndicator(strokeWidth: 2),
                  )
                else ...[
                  IconButton(
                    onPressed: _toggleFavorite,
                    tooltip: strings.favorites,
                    icon: Icon(
                      progress.isFavorite
                          ? Icons.favorite
                          : Icons.favorite_border,
                      size: 20,
                    ),
                    visualDensity: VisualDensity.compact,
                  ),
                  IconButton(
                    tooltip: strings.markSkipped,
                    onPressed: () => _setStatus(
                      progress.status == EpisodeWatchStatus.skipped
                          ? EpisodeWatchStatus.unwatched
                          : EpisodeWatchStatus.skipped,
                    ),
                    icon: Icon(
                      Icons.skip_next,
                      color: progress.status == EpisodeWatchStatus.skipped
                          ? scheme.primary
                          : null,
                      size: 20,
                    ),
                    visualDensity: VisualDensity.compact,
                  ),
                  IconButton(
                    tooltip: strings.markWatched,
                    onPressed: () => _setStatus(
                      progress.status == EpisodeWatchStatus.watched
                          ? EpisodeWatchStatus.unwatched
                          : EpisodeWatchStatus.watched,
                    ),
                    icon: Icon(
                      progress.status == EpisodeWatchStatus.watched
                          ? Icons.check_circle
                          : Icons.check_circle_outline,
                      color: progress.status == EpisodeWatchStatus.watched
                          ? scheme.primary
                          : null,
                    ),
                    visualDensity: VisualDensity.compact,
                  ),
                ],
              ],
            ),
          ],
        ),
      ),
    );
  }
}
