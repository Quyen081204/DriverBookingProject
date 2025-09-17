import { Platform, Text, View, StyleSheet } from 'react-native';
import { useState, useEffect } from 'react';
import * as Location from 'expo-location';

export const SearchLocationScreen = () => {
    

    return (
    <View style={styles.container}>
      <Text style={styles.paragraph}>Hello this is the search screen</Text>
    </View>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    alignItems: 'center',
    justifyContent: 'center',
    padding: 20,
  },
  paragraph: {
    fontSize: 18,
    textAlign: 'center',
  },
});