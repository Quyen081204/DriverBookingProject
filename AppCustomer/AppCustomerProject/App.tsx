import React from 'react'
import { StatusBar } from 'expo-status-bar';
import { StyleSheet, Text, View } from 'react-native';
import { AppNavigation } from './src/presentation/navigation/AppNavigation';
import { Provider } from 'react-redux';
import { Provider  as PaperProvider } from 'react-native-paper';
import { store } from './src/presentation/redux/store';

export default function App(): React.JSX.Element {
  return (
    <Provider store={store}>
      <PaperProvider>
        <AppNavigation />
      </PaperProvider>
    </Provider>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: '#fff',
    alignItems: 'center',
    justifyContent: 'center',
  },
});
