import 'package:cached_network_image/cached_network_image.dart';
import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';
import 'package:url_launcher/url_launcher.dart';

import '../../core/config/tmdb_images.dart';
import '../../core/localization/app_strings.dart';
import '../../core/theme/app_theme.dart';
import '../../core/widgets/async_value_view.dart';
import '../../core/widgets/poster_card.dart';
import '../../core/widgets/section_header.dart';
import 'add_to_list_sheet.dart';
import 'details_providers.dart';

class SeriesDetailsScreen extends ConsumerWidget {
  final int tmdbId;
  const SeriesDetailsScreen({super.key, required this.tmdbId});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final strings = AppStrings.of(context);
    final detail = ref.watch(seriesDetailProvider(tmdbId));

    return Scaffold(
      body: AsyncValueView(
        value: detail,
        loadingHeight: 500,
        onRetry: () => ref.invalidate(seriesDetailProvider(tmdbId)),
        data: (series) => CustomScrollView(
          slivers: [
            SliverAppBar(
              expandedHeight: 300,
              pinned: true,
              flexibleSpace: FlexibleSpaceBar(
                background: Stack(
                  fit: StackFit.expand,
                  children: [
                    if (series.backdropPath != null)
                      CachedNetworkImage(imageUrl: TmdbImages.backdrop(series.backdropPath)!, fit: BoxFit.cover)
                    else
                      Container(color: Theme.of(context).colorScheme.surfaceContainerHigh),
                    DecoratedBox(decoration: BoxDecoration(gradient: AppTheme.heroScrim(Theme.of(context).colorScheme))),
                  ],
                ),
              ),
            ),
            SliverToBoxAdapter(
              child: Padding(
                padding: const EdgeInsets.all(16),
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text(series.title, style: Theme.of(context).textTheme.headlineSmall?.copyWith(fontWeight: FontWeight.w800)),
                    const SizedBox(height: 8),
                    Wrap(
                      spacing: 12,
                      runSpacing: 4,
                      children: [
                        _MetaChip(icon: Icons.star_rounded, label: series.voteAverage.toStringAsFixed(1)),
                        _MetaChip(icon: Icons.tv, label: series.status),
                        if (series.firstAirDate != null) _MetaChip(icon: Icons.event, label: '${series.firstAirDate!.year}'),
                      ],
                    ),
                    if (series.genres.isNotEmpty) ...[
                      const SizedBox(height: 12),
                      Wrap(spacing: 8, children: series.genres.map((g) => Chip(label: Text(g))).toList()),
                    ],
                    const SizedBox(height: 16),
                    Row(
                      children: [
                        Expanded(
                          child: FilledButton.icon(
                            onPressed: series.seasons.isEmpty
                                ? null
                                : () => context.push('/series/$tmdbId/season/${series.seasons.first.seasonNumber}'),
                            icon: const Icon(Icons.play_arrow_rounded),
                            label: const Text('Start Watching'),
                          ),
                        ),
                        const SizedBox(width: 8),
                        IconButton.filledTonal(
                          onPressed: () => showAddToListSheet(context, ref, seriesTmdbId: tmdbId),
                          icon: const Icon(Icons.playlist_add),
                        ),
                      ],
                    ),
                    if (series.trailerYoutubeKey != null) ...[
                      const SizedBox(height: 8),
                      OutlinedButton.icon(
                        onPressed: () => launchUrl(Uri.parse('https://www.youtube.com/watch?v=${series.trailerYoutubeKey}'),
                            mode: LaunchMode.externalApplication),
                        icon: const Icon(Icons.play_circle_outline),
                        label: const Text('Watch Trailer'),
                      ),
                    ],
                    if (series.overview != null && series.overview!.isNotEmpty) ...[
                      const SizedBox(height: 20),
                      Text(strings.overview, style: Theme.of(context).textTheme.titleMedium?.copyWith(fontWeight: FontWeight.w700)),
                      const SizedBox(height: 6),
                      Text(series.overview!, style: Theme.of(context).textTheme.bodyMedium),
                    ],
                  ],
                ),
              ),
            ),
            if (series.seasons.isNotEmpty) ...[
              SliverToBoxAdapter(child: SectionHeader(title: strings.seasons)),
              SliverList.list(
                children: series.seasons
                    .map((season) => ListTile(
                          leading: SizedBox(
                            width: 48,
                            child: season.posterPath != null
                                ? ClipRRect(
                                    borderRadius: BorderRadius.circular(6),
                                    child: CachedNetworkImage(imageUrl: TmdbImages.poster(season.posterPath, size: 'w92')!, fit: BoxFit.cover),
                                  )
                                : const Icon(Icons.image_not_supported_outlined),
                          ),
                          title: Text(season.name),
                          subtitle: Text('${season.episodeCount} episodes'),
                          trailing: const Icon(Icons.chevron_right),
                          onTap: () => context.push('/series/$tmdbId/season/${season.seasonNumber}'),
                        ))
                    .toList(),
              ),
            ],
            if (series.cast.isNotEmpty) ...[
              SliverToBoxAdapter(child: SectionHeader(title: strings.cast)),
              SliverToBoxAdapter(
                child: SizedBox(
                  height: 130,
                  child: ListView.separated(
                    scrollDirection: Axis.horizontal,
                    padding: const EdgeInsets.symmetric(horizontal: 16),
                    itemCount: series.cast.length,
                    separatorBuilder: (context, index) => const SizedBox(width: 12),
                    itemBuilder: (context, index) {
                      final member = series.cast[index];
                      return SizedBox(
                        width: 84,
                        child: Column(
                          children: [
                            CircleAvatar(
                              radius: 36,
                              backgroundImage:
                                  member.profilePath != null ? NetworkImage(TmdbImages.profile(member.profilePath)!) : null,
                              child: member.profilePath == null ? const Icon(Icons.person) : null,
                            ),
                            const SizedBox(height: 6),
                            Text(member.name, maxLines: 1, overflow: TextOverflow.ellipsis, textAlign: TextAlign.center, style: Theme.of(context).textTheme.bodySmall),
                          ],
                        ),
                      );
                    },
                  ),
                ),
              ),
            ],
            SliverToBoxAdapter(child: SectionHeader(title: strings.similar)),
            SliverToBoxAdapter(
              child: SizedBox(
                height: 250,
                child: Consumer(
                  builder: (context, ref, child) {
                    final similar = ref.watch(similarSeriesProvider(tmdbId));
                    return AsyncValueView(
                      value: similar,
                      data: (result) => ListView.separated(
                        scrollDirection: Axis.horizontal,
                        padding: const EdgeInsets.symmetric(horizontal: 16),
                        itemCount: result.items.length,
                        separatorBuilder: (context, index) => const SizedBox(width: 12),
                        itemBuilder: (context, index) {
                          final s = result.items[index];
                          return PosterCard(
                            posterUrl: TmdbImages.poster(s.posterPath),
                            title: s.title,
                            rating: s.voteAverage,
                            onTap: () => context.push('/series/${s.tmdbId}'),
                          );
                        },
                      ),
                    );
                  },
                ),
              ),
            ),
            const SliverToBoxAdapter(child: SizedBox(height: 24)),
          ],
        ),
      ),
    );
  }
}

class _MetaChip extends StatelessWidget {
  final IconData icon;
  final String label;
  const _MetaChip({required this.icon, required this.label});

  @override
  Widget build(BuildContext context) {
    return Row(
      mainAxisSize: MainAxisSize.min,
      children: [
        Icon(icon, size: 16, color: AppTheme.accent),
        const SizedBox(width: 4),
        Text(label, style: Theme.of(context).textTheme.bodyMedium),
      ],
    );
  }
}
