import 'package:flutter/material.dart';

import 'case_repository.dart';
import 'case_summary.dart';

class CaseSelectionScreen extends StatefulWidget {
  const CaseSelectionScreen({required this.repository, super.key});

  final CaseRepository repository;

  @override
  State<CaseSelectionScreen> createState() => _CaseSelectionScreenState();
}

class _CaseSelectionScreenState extends State<CaseSelectionScreen> {
  late Future<List<CaseSummary>> _cases;

  @override
  void initState() {
    super.initState();
    _cases = widget.repository.listCases();
  }

  void _retry() {
    setState(() {
      _cases = widget.repository.listCases();
    });
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: const Text('Questigma')),
      body: FutureBuilder<List<CaseSummary>>(
        future: _cases,
        builder: (context, snapshot) {
          if (snapshot.connectionState != ConnectionState.done) {
            return const Center(child: CircularProgressIndicator());
          }
          if (snapshot.hasError) {
            return Center(
              child: Padding(
                padding: const EdgeInsets.all(24),
                child: Column(
                  mainAxisSize: MainAxisSize.min,
                  children: [
                    const Text(
                      'Não foi possível carregar os casos. Verifique sua conexão e tente novamente.',
                      textAlign: TextAlign.center,
                    ),
                    const SizedBox(height: 16),
                    FilledButton(
                      onPressed: _retry,
                      child: const Text('Tentar novamente'),
                    ),
                  ],
                ),
              ),
            );
          }
          final cases = snapshot.data ?? [];
          if (cases.isEmpty) {
            return const Center(
              child: Text('Nenhum caso disponível no momento.'),
            );
          }
          return ListView.builder(
            padding: const EdgeInsets.all(16),
            itemCount: cases.length,
            itemBuilder: (context, index) {
              final item = cases[index];
              return Card(
                clipBehavior: Clip.antiAlias,
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Container(
                      height: 140,
                      width: double.infinity,
                      color: Theme.of(context).colorScheme.primaryContainer,
                      child: Icon(
                        Icons.menu_book_outlined,
                        size: 64,
                        color: Theme.of(context).colorScheme.onPrimaryContainer,
                      ),
                    ),
                    Padding(
                      padding: const EdgeInsets.all(16),
                      child: Column(
                        crossAxisAlignment: CrossAxisAlignment.start,
                        children: [
                          Text(
                            item.title,
                            style: Theme.of(context).textTheme.titleLarge,
                          ),
                          const SizedBox(height: 8),
                          Text(item.synopsis),
                        ],
                      ),
                    ),
                  ],
                ),
              );
            },
          );
        },
      ),
    );
  }
}
