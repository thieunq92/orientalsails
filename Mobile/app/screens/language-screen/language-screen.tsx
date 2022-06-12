/* eslint-disable react-native/sort-styles */
import React from "react"
import { observer } from "mobx-react-lite"
import { Image, StyleSheet, ViewStyle } from "react-native"
import { Button, Screen, Text } from "../../components"
// import { useNavigation } from "@react-navigation/native"
// import { useStores } from "../../models"
import { color } from "../../theme"

const ROOT: ViewStyle = {
  backgroundColor: color.palette.offWhite,
  flex: 1,
}

export const LanguageScreen = observer(function LanguageScreen() {
  // Pull in one of our MST stores
  // const { someStore, anotherStore } = useStores()
  // OR
  // const rootStore = useStores()

  // Pull in navigation via hook
  // const navigation = useNavigation()

  const styles = StyleSheet.create({
    headerImage: {
      marginTop: 0,
      marginRight: "auto",
      marginBottom: 0,
      marginLeft: "auto",
      width: 280,
      height: 121
    }
  })

  return (
    <Screen style={ROOT} preset="scroll">
      <Image style={styles.headerImage} source={{ uri: 'https://www.orientalsails.com/wp-content/themes/orientalsails2/img/logo.png' }}/>
    </Screen>
  )
})
