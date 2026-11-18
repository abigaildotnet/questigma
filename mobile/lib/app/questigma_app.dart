import 'package:flutter/material.dart';

class QuestigmaApp extends StatelessWidget {
  const QuestigmaApp({super.key});

  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      title: 'Questigma',
      home: Scaffold(
        appBar: AppBar(title: const Text('Questigma')),
        body: const Center(child: Text('Questigma')),
      ),
    );
  }
}
