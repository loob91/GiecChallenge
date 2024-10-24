import React from "react";
import { useTranslation } from "react-i18next";

const LanguageSelector = () => {
  const { i18n } = useTranslation();

  const changeLanguage = (event) => {
    i18n.changeLanguage(event.target.value);
  };


  return (
    <div onChange={changeLanguage}>
      <select name="language">
        <option value="FR">Français</option>
        <option value="EN">English</option>
      </select>
    </div>
  );
};

export default LanguageSelector;