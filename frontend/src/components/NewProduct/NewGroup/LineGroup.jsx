import React from 'react';
import { useEffect, useState } from 'react';
import { useTranslation } from 'react-i18next';

export const LineGroup = ({ nameSelect, languageOptions, onChange }) => {
    const [group, setGroup] = useState('')
    const [language, setLanguage] = useState('')
    const { t } = useTranslation(["NewProduct"]);

    useEffect(() => {  
      onChange({
        group,
        language,
        nameSelect
      });
    }, [onChange, group, language]);
    
    return (
      <>
        <div className="form-group">
            <label htmlFor="command">{t("group")}</label>
            <input type="language" name="group" id="group" className='form-control width-100' value={group} placeholder={t("group")} onChange={(e) => setGroup(e.target.value)} />
            <label htmlFor="language">{t("language")}</label>
            <select name="language" id="language" value={language} className="form-control width-100" onChange={(e) => setLanguage(e.target.value)}>
                {languageOptions}
            </select>
        </div>
      </>
    );
  };