import React from 'react';
import { useEffect, useState } from 'react';
import { useTranslation } from 'react-i18next';

export const LineSubGroup = ({ nameSelect, languageOptions, onChange }) => {
    const [subGroup, setSubGroup] = useState('')
    const [language, setLanguage] = useState('')
    const { t } = useTranslation(["NewProduct"]);

    useEffect(() => {  
      onChange({
        subGroup,
        language,
        nameSelect
      });
    }, [onChange, subGroup, language]);
    
    return (
      <>
        <div className="form-group">
            <label htmlFor="command">{t("subGroup")}</label>
            <input type="text" name="subgroup" id="subgroup" className='form-control width-100' value={subGroup} placeholder={t("subGroup")} onChange={(e) => setSubGroup(e.target.value)} />
            <label htmlFor="language">{t("language")}</label>
            <select name="language" id="language" value={language} className="form-control width-100" onChange={(e) => setLanguage(e.target.value)}>
                {languageOptions}
            </select>
        </div>
      </>
    );
  };