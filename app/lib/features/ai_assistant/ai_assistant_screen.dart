import 'package:cached_network_image/cached_network_image.dart';
import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import '../../core/config/tmdb_images.dart';
import '../../core/localization/app_strings.dart';
import '../../core/models/ai_models.dart';
import '../../core/network/api_exception.dart';
import '../../core/providers/core_providers.dart';
import '../../core/theme/app_theme.dart';

class AiAssistantScreen extends ConsumerStatefulWidget {
  const AiAssistantScreen({super.key});

  @override
  ConsumerState<AiAssistantScreen> createState() => _AiAssistantScreenState();
}

class _ChatEntry {
  final String prompt;
  final AiResponse? response;
  final String? error;
  final bool isLoading;
  _ChatEntry({required this.prompt, this.response, this.error, this.isLoading = false});
}

class _AiAssistantScreenState extends ConsumerState<AiAssistantScreen> {
  final _controller = TextEditingController();
  final _scrollController = ScrollController();
  final List<_ChatEntry> _entries = [];

  @override
  void dispose() {
    _controller.dispose();
    _scrollController.dispose();
    super.dispose();
  }

  Future<void> _ask() async {
    final prompt = _controller.text.trim();
    if (prompt.isEmpty) return;
    _controller.clear();

    setState(() => _entries.add(_ChatEntry(prompt: prompt, isLoading: true)));
    _scrollToBottom();

    try {
      final response = await ref.read(aiApiProvider).ask(prompt);
      setState(() {
        _entries[_entries.length - 1] = _ChatEntry(prompt: prompt, response: response);
      });
    } catch (e) {
      setState(() {
        _entries[_entries.length - 1] =
            _ChatEntry(prompt: prompt, error: e is ApiException ? e.message : 'Something went wrong.');
      });
    }
    _scrollToBottom();
  }

  void _scrollToBottom() {
    WidgetsBinding.instance.addPostFrameCallback((_) {
      if (_scrollController.hasClients) {
        _scrollController.animateTo(_scrollController.position.maxScrollExtent,
            duration: const Duration(milliseconds: 250), curve: Curves.easeOut);
      }
    });
  }

  @override
  Widget build(BuildContext context) {
    final strings = AppStrings.of(context);

    return Scaffold(
      appBar: AppBar(title: Text(strings.aiAssistant)),
      body: Column(
        children: [
          Expanded(
            child: _entries.isEmpty
                ? Center(
                    child: Padding(
                      padding: const EdgeInsets.all(32),
                      child: Text(strings.aiAssistantHint, textAlign: TextAlign.center, style: Theme.of(context).textTheme.bodyLarge),
                    ),
                  )
                : ListView.builder(
                    controller: _scrollController,
                    padding: const EdgeInsets.all(16),
                    itemCount: _entries.length,
                    itemBuilder: (context, index) => _ChatBubble(entry: _entries[index]),
                  ),
          ),
          SafeArea(
            top: false,
            child: Padding(
              padding: const EdgeInsets.fromLTRB(12, 8, 12, 12),
              child: Row(
                children: [
                  Expanded(
                    child: TextField(
                      controller: _controller,
                      onSubmitted: (_) => _ask(),
                      decoration: InputDecoration(hintText: strings.aiAssistantHint),
                    ),
                  ),
                  const SizedBox(width: 8),
                  IconButton.filled(onPressed: _ask, icon: const Icon(Icons.send_rounded)),
                ],
              ),
            ),
          ),
        ],
      ),
    );
  }
}

class _ChatBubble extends StatelessWidget {
  final _ChatEntry entry;
  const _ChatBubble({required this.entry});

  @override
  Widget build(BuildContext context) {
    final scheme = Theme.of(context).colorScheme;
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Align(
          alignment: Alignment.centerRight,
          child: Container(
            margin: const EdgeInsets.only(bottom: 8),
            padding: const EdgeInsets.symmetric(horizontal: 14, vertical: 10),
            constraints: BoxConstraints(maxWidth: MediaQuery.of(context).size.width * 0.75),
            decoration: BoxDecoration(color: scheme.primary, borderRadius: BorderRadius.circular(AppTheme.radiusMd)),
            child: Text(entry.prompt, style: TextStyle(color: scheme.onPrimary)),
          ),
        ),
        if (entry.isLoading)
          const Padding(
            padding: EdgeInsets.only(bottom: 16),
            child: SizedBox(width: 20, height: 20, child: CircularProgressIndicator(strokeWidth: 2)),
          )
        else if (entry.error != null)
          Padding(
            padding: const EdgeInsets.only(bottom: 16),
            child: Text(entry.error!, style: TextStyle(color: scheme.error)),
          )
        else if (entry.response != null) ...[
          Container(
            margin: const EdgeInsets.only(bottom: 12),
            padding: const EdgeInsets.symmetric(horizontal: 14, vertical: 10),
            constraints: BoxConstraints(maxWidth: MediaQuery.of(context).size.width * 0.85),
            decoration: BoxDecoration(color: scheme.surfaceContainerHigh, borderRadius: BorderRadius.circular(AppTheme.radiusMd)),
            child: Text(entry.response!.message),
          ),
          if (entry.response!.suggestions.isNotEmpty)
            SizedBox(
              height: 210,
              child: ListView.separated(
                scrollDirection: Axis.horizontal,
                itemCount: entry.response!.suggestions.length,
                separatorBuilder: (context, index) => const SizedBox(width: 10),
                itemBuilder: (context, index) => _SuggestionCard(suggestion: entry.response!.suggestions[index]),
              ),
            ),
          const SizedBox(height: 12),
        ],
      ],
    );
  }
}

class _SuggestionCard extends StatelessWidget {
  final AiSuggestion suggestion;
  const _SuggestionCard({required this.suggestion});

  @override
  Widget build(BuildContext context) {
    return SizedBox(
      width: 120,
      child: InkWell(
        borderRadius: BorderRadius.circular(AppTheme.radiusMd),
        onTap: () => context.push(suggestion.isSeries ? '/series/${suggestion.tmdbId}' : '/movie/${suggestion.tmdbId}'),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            AspectRatio(
              aspectRatio: 2 / 3,
              child: ClipRRect(
                borderRadius: BorderRadius.circular(AppTheme.radiusMd),
                child: suggestion.posterPath != null
                    ? CachedNetworkImage(imageUrl: TmdbImages.poster(suggestion.posterPath)!, fit: BoxFit.cover)
                    : Container(color: Theme.of(context).colorScheme.surfaceContainerHigh, child: const Icon(Icons.movie_outlined)),
              ),
            ),
            const SizedBox(height: 4),
            Text(suggestion.title, maxLines: 1, overflow: TextOverflow.ellipsis, style: Theme.of(context).textTheme.bodySmall?.copyWith(fontWeight: FontWeight.w600)),
            Text(suggestion.reason, maxLines: 2, overflow: TextOverflow.ellipsis, style: Theme.of(context).textTheme.labelSmall),
          ],
        ),
      ),
    );
  }
}
