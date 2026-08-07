import 'package:cached_network_image/cached_network_image.dart';
import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import '../../core/config/tmdb_images.dart';
import '../../core/localization/app_strings.dart';
import '../../core/models/stats_models.dart';
import '../../core/models/user_models.dart';
import '../../core/theme/app_theme.dart';
import '../../core/widgets/async_value_view.dart';
import '../auth/auth_provider.dart';
import '../stats/stats_providers.dart';
import 'profile_providers.dart';

class ProfileScreen extends ConsumerWidget {
  const ProfileScreen({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final strings = AppStrings.of(context);
    final auth = ref.watch(authProvider);
    final profile = auth.profile;
    final lists = ref.watch(myListsProvider);
    final stats = ref.watch(myStatsProvider);

    return Scaffold(
      appBar: AppBar(
        title: Text(
          strings.navProfile,
          style: const TextStyle(fontWeight: FontWeight.w800),
        ),
        actions: [
          IconButton(
            icon: const Icon(Icons.settings_outlined),
            onPressed: () => context.push('/settings'),
          ),
        ],
      ),
      body: ListView(
        padding: const EdgeInsets.fromLTRB(16, 12, 16, 24),
        children: [
          if (profile != null) _ProfileHero(profile: profile),
          if (profile?.isAdmin == true) ...[
            const SizedBox(height: 12),
            _AdminAccessCard(onTap: () => context.push('/admin')),
          ],
          const SizedBox(height: 20),
          AsyncValueView(
            value: stats,
            loadingHeight: 210,
            onRetry: () => ref.invalidate(myStatsProvider),
            data: (data) => Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Row(
                  children: [
                    Expanded(
                      child: _ProfileMetricCard(
                        label: strings.totalMovies,
                        value: '${data.totalMoviesWatched}',
                        icon: Icons.movie_outlined,
                      ),
                    ),
                    const SizedBox(width: 12),
                    Expanded(
                      child: _ProfileMetricCard(
                        label: strings.totalEpisodes,
                        value: '${data.totalEpisodesWatched}',
                        icon: Icons.live_tv_outlined,
                      ),
                    ),
                  ],
                ),
                const SizedBox(height: 12),
                _ProfileMetricCard(
                  label: strings.totalWatchTime,
                  value: _formatMinutes(data.totalWatchTimeMinutes),
                  icon: Icons.schedule_outlined,
                  fullWidth: true,
                ),
                if (data.achievements.isNotEmpty) ...[
                  const SizedBox(height: 20),
                  Row(
                    children: [
                      Expanded(
                        child: Text(
                          strings.achievements,
                          style: Theme.of(context).textTheme.titleMedium
                              ?.copyWith(fontWeight: FontWeight.w800),
                        ),
                      ),
                      TextButton(
                        onPressed: () => context.go('/stats'),
                        child: Text(strings.viewAll),
                      ),
                    ],
                  ),
                  const SizedBox(height: 10),
                  Wrap(
                    spacing: 10,
                    runSpacing: 10,
                    children: data.achievements
                        .take(4)
                        .map((badge) => _ProfileBadge(badge: badge))
                        .toList(),
                  ),
                ],
              ],
            ),
          ),
          const SizedBox(height: 24),
          Text(
            strings.lists,
            style: Theme.of(
              context,
            ).textTheme.titleMedium?.copyWith(fontWeight: FontWeight.w800),
          ),
          const SizedBox(height: 8),
          AsyncValueView(
            value: lists,
            loadingHeight: 150,
            onRetry: () => ref.invalidate(myListsProvider),
            data: (userLists) => Column(
              children: userLists
                  .map((list) => _ListExpansion(list: list))
                  .toList(),
            ),
          ),
        ],
      ),
    );
  }

  String _formatMinutes(int minutes) {
    final hours = minutes ~/ 60;
    final mins = minutes % 60;
    return '${hours}h ${mins}m';
  }
}

class _ListExpansion extends ConsumerWidget {
  final dynamic list;
  const _ListExpansion({required this.list});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    return Card(
      margin: const EdgeInsets.only(bottom: 8),
      child: ExpansionTile(
        title: Text(list.name as String),
        subtitle: Text('${list.itemCount} items'),
        shape: RoundedRectangleBorder(
          borderRadius: BorderRadius.circular(AppTheme.radiusMd),
        ),
        children: [
          if ((list.itemCount as int) == 0)
            const Padding(
              padding: EdgeInsets.all(16),
              child: Text('No items yet.'),
            )
          else
            Consumer(
              builder: (context, ref, _) {
                final items = ref.watch(listItemsProvider(list.id as String));
                return AsyncValueView(
                  value: items,
                  loadingHeight: 80,
                  data: (rows) => Column(
                    children: rows
                        .map(
                          (item) => ListTile(
                            leading: item.posterPath != null
                                ? ClipRRect(
                                    borderRadius: BorderRadius.circular(6),
                                    child: CachedNetworkImage(
                                      imageUrl: TmdbImages.poster(
                                        item.posterPath,
                                        size: 'w92',
                                      )!,
                                      width: 40,
                                      fit: BoxFit.cover,
                                    ),
                                  )
                                : const Icon(
                                    Icons.image_not_supported_outlined,
                                  ),
                            title: Text(item.title),
                            onTap: () => context.push(
                              item.isMovie
                                  ? '/movie/${item.movieTmdbId}'
                                  : '/series/${item.seriesTmdbId}',
                            ),
                          ),
                        )
                        .toList(),
                  ),
                );
              },
            ),
        ],
      ),
    );
  }
}

class _ProfileHero extends StatelessWidget {
  final MeProfile profile;

  const _ProfileHero({required this.profile});

  @override
  Widget build(BuildContext context) {
    final strings = AppStrings.of(context);
    final scheme = Theme.of(context).colorScheme;

    return Container(
      padding: const EdgeInsets.all(20),
      decoration: BoxDecoration(
        gradient: LinearGradient(
          begin: Alignment.topLeft,
          end: Alignment.bottomRight,
          colors: [scheme.primaryContainer, scheme.tertiaryContainer],
        ),
        borderRadius: BorderRadius.circular(AppTheme.radiusLg),
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Row(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              CircleAvatar(
                radius: 36,
                backgroundColor: scheme.surface,
                backgroundImage: profile.avatarUrl != null
                    ? NetworkImage(profile.avatarUrl!)
                    : null,
                child: profile.avatarUrl == null
                    ? Text(
                        (profile.displayName.isNotEmpty
                                ? profile.displayName[0]
                                : '?')
                            .toUpperCase(),
                        style: TextStyle(
                          fontSize: 26,
                          color: scheme.primary,
                          fontWeight: FontWeight.w800,
                        ),
                      )
                    : null,
              ),
              const SizedBox(width: 16),
              Expanded(
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text(
                      profile.displayName,
                      style: Theme.of(context).textTheme.headlineSmall
                          ?.copyWith(fontWeight: FontWeight.w900),
                    ),
                    const SizedBox(height: 4),
                    Text(
                      profile.email,
                      style: Theme.of(context).textTheme.bodyMedium?.copyWith(
                        color: scheme.onSurfaceVariant,
                      ),
                    ),
                    const SizedBox(height: 10),
                    Wrap(
                      spacing: 8,
                      runSpacing: 8,
                      children: [
                        _HeroChip(
                          label:
                              '${profile.followerCount} ${strings.followers}',
                        ),
                        _HeroChip(
                          label:
                              '${profile.followingCount} ${strings.following}',
                        ),
                        _HeroChip(
                          label: '${strings.joined} ${profile.createdAt.year}',
                        ),
                      ],
                    ),
                  ],
                ),
              ),
            ],
          ),
          if (profile.bio != null && profile.bio!.isNotEmpty) ...[
            const SizedBox(height: 16),
            Text(
              profile.bio!,
              style: Theme.of(
                context,
              ).textTheme.bodyLarge?.copyWith(height: 1.4),
            ),
          ],
        ],
      ),
    );
  }
}

class _AdminAccessCard extends StatelessWidget {
  final VoidCallback onTap;

  const _AdminAccessCard({required this.onTap});

  @override
  Widget build(BuildContext context) {
    final strings = AppStrings.of(context);
    final scheme = Theme.of(context).colorScheme;

    return Card(
      child: ListTile(
        contentPadding: const EdgeInsets.symmetric(
          horizontal: 16,
          vertical: 10,
        ),
        leading: Container(
          padding: const EdgeInsets.all(10),
          decoration: BoxDecoration(
            color: scheme.primaryContainer,
            borderRadius: BorderRadius.circular(AppTheme.radiusSm),
          ),
          child: Icon(
            Icons.admin_panel_settings_outlined,
            color: scheme.onPrimaryContainer,
          ),
        ),
        title: Text(
          strings.adminTools,
          style: Theme.of(
            context,
          ).textTheme.titleSmall?.copyWith(fontWeight: FontWeight.w800),
        ),
        subtitle: Text(strings.openAdminTools),
        trailing: const Icon(Icons.chevron_right_rounded),
        onTap: onTap,
      ),
    );
  }
}

class _HeroChip extends StatelessWidget {
  final String label;

  const _HeroChip({required this.label});

  @override
  Widget build(BuildContext context) {
    final scheme = Theme.of(context).colorScheme;

    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 8),
      decoration: BoxDecoration(
        color: scheme.surface.withValues(alpha: 0.78),
        borderRadius: BorderRadius.circular(AppTheme.radiusSm * 2),
      ),
      child: Text(
        label,
        style: Theme.of(
          context,
        ).textTheme.bodySmall?.copyWith(fontWeight: FontWeight.w700),
      ),
    );
  }
}

class _ProfileMetricCard extends StatelessWidget {
  final String label;
  final String value;
  final IconData icon;
  final bool fullWidth;

  const _ProfileMetricCard({
    required this.label,
    required this.value,
    required this.icon,
    this.fullWidth = false,
  });

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
              decoration: BoxDecoration(
                color: scheme.primaryContainer,
                borderRadius: BorderRadius.circular(AppTheme.radiusSm),
              ),
              child: Icon(icon, color: scheme.onPrimaryContainer),
            ),
            const SizedBox(width: 12),
            Expanded(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(
                    value,
                    style: Theme.of(context).textTheme.titleLarge?.copyWith(
                      fontWeight: FontWeight.w900,
                    ),
                  ),
                  Text(
                    label,
                    style: Theme.of(context).textTheme.bodySmall?.copyWith(
                      color: scheme.onSurfaceVariant,
                    ),
                  ),
                ],
              ),
            ),
            if (fullWidth)
              Icon(Icons.trending_up_rounded, color: scheme.primary),
          ],
        ),
      ),
    );
  }
}

class _ProfileBadge extends StatelessWidget {
  final AchievementBadge badge;

  const _ProfileBadge({required this.badge});

  @override
  Widget build(BuildContext context) {
    final scheme = Theme.of(context).colorScheme;

    return Tooltip(
      message: badge.description,
      child: Container(
        padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 10),
        decoration: BoxDecoration(
          color: scheme.tertiaryContainer,
          borderRadius: BorderRadius.circular(AppTheme.radiusSm * 2),
        ),
        child: Row(
          mainAxisSize: MainAxisSize.min,
          children: [
            Icon(
              Icons.workspace_premium_rounded,
              size: 18,
              color: scheme.onTertiaryContainer,
            ),
            const SizedBox(width: 8),
            Text(
              badge.name,
              style: TextStyle(
                color: scheme.onTertiaryContainer,
                fontWeight: FontWeight.w700,
              ),
            ),
          ],
        ),
      ),
    );
  }
}
