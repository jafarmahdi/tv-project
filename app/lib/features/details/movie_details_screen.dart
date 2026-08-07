import 'package:cached_network_image/cached_network_image.dart';
import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';
import 'package:url_launcher/url_launcher.dart';

import '../../core/config/tmdb_images.dart';
import '../../core/localization/app_strings.dart';
import '../../core/network/api_exception.dart';
import '../../core/providers/core_providers.dart';
import '../../core/theme/app_theme.dart';
import '../../core/widgets/async_value_view.dart';
import '../../core/widgets/poster_card.dart';
import '../../core/widgets/section_header.dart';
import '../engagement/engagement_section.dart';
import '../../core/models/engagement_models.dart';
import 'add_to_list_sheet.dart';
import 'details_providers.dart';

class MovieDetailsScreen extends ConsumerStatefulWidget {
  final int tmdbId;
  const MovieDetailsScreen({super.key, required this.tmdbId});

  @override
  ConsumerState<MovieDetailsScreen> createState() => _MovieDetailsScreenState();
}

class _MovieDetailsScreenState extends ConsumerState<MovieDetailsScreen> {
  bool _isWatched = false;
  bool _isFavorite = false;
  bool _isSubmittingWatched = false;
  bool _isSubmittingFavorite = false;

  Future<void> _toggleWatched() async {
    setState(() => _isSubmittingWatched = true);
    try {
      await ref
          .read(trackingApiProvider)
          .markMovie(movieTmdbId: widget.tmdbId, isWatched: !_isWatched);
      setState(() => _isWatched = !_isWatched);
    } catch (e) {
      _showError(e);
    } finally {
      if (mounted) setState(() => _isSubmittingWatched = false);
    }
  }

  Future<void> _toggleFavorite() async {
    setState(() => _isSubmittingFavorite = true);
    try {
      await ref
          .read(trackingApiProvider)
          .toggleMovieFavorite(
            movieTmdbId: widget.tmdbId,
            isFavorite: !_isFavorite,
          );
      setState(() => _isFavorite = !_isFavorite);
    } catch (e) {
      _showError(e);
    } finally {
      if (mounted) setState(() => _isSubmittingFavorite = false);
    }
  }

  void _showError(Object e) {
    if (!mounted) return;
    ScaffoldMessenger.of(context).showSnackBar(
      SnackBar(
        content: Text(e is ApiException ? e.message : 'Something went wrong.'),
      ),
    );
  }

  @override
  Widget build(BuildContext context) {
    final strings = AppStrings.of(context);
    final detail = ref.watch(movieDetailProvider(widget.tmdbId));

    return Scaffold(
      body: AsyncValueView(
        value: detail,
        loadingHeight: 500,
        onRetry: () => ref.invalidate(movieDetailProvider(widget.tmdbId)),
        data: (movie) => CustomScrollView(
          slivers: [
            SliverAppBar(
              expandedHeight: 300,
              pinned: true,
              flexibleSpace: FlexibleSpaceBar(
                background: Stack(
                  fit: StackFit.expand,
                  children: [
                    if (movie.backdropPath != null)
                      CachedNetworkImage(
                        imageUrl: TmdbImages.backdrop(movie.backdropPath)!,
                        fit: BoxFit.cover,
                      )
                    else
                      Container(
                        color: Theme.of(
                          context,
                        ).colorScheme.surfaceContainerHigh,
                      ),
                    DecoratedBox(
                      decoration: BoxDecoration(
                        gradient: AppTheme.heroScrim(
                          Theme.of(context).colorScheme,
                        ),
                      ),
                    ),
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
                    Text(
                      movie.title,
                      style: Theme.of(context).textTheme.headlineSmall
                          ?.copyWith(fontWeight: FontWeight.w800),
                    ),
                    const SizedBox(height: 8),
                    Wrap(
                      spacing: 12,
                      runSpacing: 4,
                      children: [
                        _MetaChip(
                          icon: Icons.star_rounded,
                          label: movie.voteAverage.toStringAsFixed(1),
                        ),
                        if (movie.runtimeMinutes != null)
                          _MetaChip(
                            icon: Icons.schedule,
                            label: '${movie.runtimeMinutes} min',
                          ),
                        if (movie.releaseDate != null)
                          _MetaChip(
                            icon: Icons.event,
                            label: '${movie.releaseDate!.year}',
                          ),
                      ],
                    ),
                    if (movie.genres.isNotEmpty) ...[
                      const SizedBox(height: 12),
                      Wrap(
                        spacing: 8,
                        children: movie.genres
                            .map((g) => Chip(label: Text(g)))
                            .toList(),
                      ),
                    ],
                    const SizedBox(height: 16),
                    Row(
                      children: [
                        Expanded(
                          child: FilledButton.icon(
                            onPressed: _isSubmittingWatched
                                ? null
                                : _toggleWatched,
                            icon: Icon(
                              _isWatched
                                  ? Icons.check_circle
                                  : Icons.check_circle_outline,
                            ),
                            label: Text(
                              _isWatched ? 'Watched' : strings.markWatched,
                            ),
                          ),
                        ),
                        const SizedBox(width: 8),
                        IconButton.filledTonal(
                          onPressed: _isSubmittingFavorite
                              ? null
                              : _toggleFavorite,
                          icon: Icon(
                            _isFavorite
                                ? Icons.favorite
                                : Icons.favorite_border,
                          ),
                        ),
                        const SizedBox(width: 8),
                        IconButton.filledTonal(
                          onPressed: () => showAddToListSheet(
                            context,
                            ref,
                            movieTmdbId: widget.tmdbId,
                          ),
                          icon: const Icon(Icons.playlist_add),
                        ),
                      ],
                    ),
                    if (movie.trailerYoutubeKey != null) ...[
                      const SizedBox(height: 8),
                      OutlinedButton.icon(
                        onPressed: () => launchUrl(
                          Uri.parse(
                            'https://www.youtube.com/watch?v=${movie.trailerYoutubeKey}',
                          ),
                          mode: LaunchMode.externalApplication,
                        ),
                        icon: const Icon(Icons.play_circle_outline),
                        label: const Text('Watch Trailer'),
                      ),
                    ],
                    if (movie.overview != null &&
                        movie.overview!.isNotEmpty) ...[
                      const SizedBox(height: 20),
                      Text(
                        strings.overview,
                        style: Theme.of(context).textTheme.titleMedium
                            ?.copyWith(fontWeight: FontWeight.w700),
                      ),
                      const SizedBox(height: 6),
                      Text(
                        movie.overview!,
                        style: Theme.of(context).textTheme.bodyMedium,
                      ),
                    ],
                    const SizedBox(height: 24),
                    EngagementSection(
                      targetType: TargetType.movie,
                      targetId: movie.id,
                      title: strings.community,
                    ),
                  ],
                ),
              ),
            ),
            if (movie.cast.isNotEmpty) ...[
              SliverToBoxAdapter(child: SectionHeader(title: strings.cast)),
              SliverToBoxAdapter(
                child: SizedBox(
                  height: 130,
                  child: ListView.separated(
                    scrollDirection: Axis.horizontal,
                    padding: const EdgeInsets.symmetric(horizontal: 16),
                    itemCount: movie.cast.length,
                    separatorBuilder: (_, _) => const SizedBox(width: 12),
                    itemBuilder: (context, index) {
                      final member = movie.cast[index];
                      return SizedBox(
                        width: 84,
                        child: Column(
                          children: [
                            CircleAvatar(
                              radius: 36,
                              backgroundImage: member.profilePath != null
                                  ? NetworkImage(
                                      TmdbImages.profile(member.profilePath)!,
                                    )
                                  : null,
                              child: member.profilePath == null
                                  ? const Icon(Icons.person)
                                  : null,
                            ),
                            const SizedBox(height: 6),
                            Text(
                              member.name,
                              maxLines: 1,
                              overflow: TextOverflow.ellipsis,
                              textAlign: TextAlign.center,
                              style: Theme.of(context).textTheme.bodySmall,
                            ),
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
                  builder: (context, ref, _) {
                    final similar = ref.watch(
                      similarMoviesProvider(widget.tmdbId),
                    );
                    return AsyncValueView(
                      value: similar,
                      data: (result) => ListView.separated(
                        scrollDirection: Axis.horizontal,
                        padding: const EdgeInsets.symmetric(horizontal: 16),
                        itemCount: result.items.length,
                        separatorBuilder: (_, _) => const SizedBox(width: 12),
                        itemBuilder: (context, index) {
                          final m = result.items[index];
                          return PosterCard(
                            posterUrl: TmdbImages.poster(m.posterPath),
                            title: m.title,
                            rating: m.voteAverage,
                            onTap: () => context.push('/movie/${m.tmdbId}'),
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
