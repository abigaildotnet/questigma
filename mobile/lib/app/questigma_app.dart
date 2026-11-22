import 'package:flutter/material.dart';
import '../cases/case_repository.dart';
import '../cases/case_selection_screen.dart';

class QuestigmaApp extends StatelessWidget {
  const QuestigmaApp({required this.repository, super.key});

  final CaseRepository repository;

  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      title: 'Questigma',
      home: CaseSelectionScreen(repository: repository),
    );
  }
}
