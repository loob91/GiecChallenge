import React from 'react'
import {FaArrowCircleLeft} from 'react-icons/fa'
import {Link} from 'react-router-dom'
import { useTranslation } from 'react-i18next';

const BackButton = ({url}) => {
  const { t } = useTranslation(["Purchase"]);

  return (
      <Link className="btn btn-reverse btn-black" to={url}>
          <FaArrowCircleLeft /> {t("back")}
      </Link>
  )
}

export default BackButton